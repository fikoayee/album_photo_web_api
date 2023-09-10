using album_photo_web_api.Data;
using album_photo_web_api.Data.ViewModels.Authentication;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace album_photo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }
        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromForm] RegisterVM register)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please provide all required fields");

            var userExists = await _userManager.FindByNameAsync(register.UserName);

            if (userExists != null)
            {
                return BadRequest($"User {register.UserName} already exists.");
            }

            User newUser = new User()
            {
                UserName = register.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(newUser, register.Password);

            if (!result.Succeeded)
            {
                return BadRequest("User could not be created!");
            }

            return Created(nameof(Register), $"User {register.UserName} created.");
        }

        private async Task<AuthenticationResultVM> GenerateJwtTokenAsync(User user, string existingRefreshToken)
        {
            string roleType = "USER";
            if(user.UserName == "admin")
            {
                roleType = "ADMIN";
            }
            
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, roleType),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.Now,
                    DateExpire = DateTime.Now.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
                };

                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
            

            var response = new AuthenticationResultVM()
            {
                Token = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken)) ? refreshToken.Token : existingRefreshToken,
                ExpiresAt = token.ValidTo
            };

            return response;
        }
        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromForm] LoginVM login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please provide all required fields");

            var user = await _userManager.FindByNameAsync(login.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user, "");
                return Ok(tokenValue);
            }

            return Unauthorized();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromForm] TokenRequestVM tokenReq)
        {
            try
            {
                var result = await VerifyAndGenerateTokenAsync(tokenReq);

                if (result == null) return BadRequest("Invalid tokens");
                else
                    return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<AuthenticationResultVM> VerifyAndGenerateTokenAsync(TokenRequestVM tokenReq)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // checks JWT's token format
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenReq.Token, _tokenValidationParameters, out var validatedToken);

                // encryption algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                        return null;
                }

                // checks validate expire date
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = UnixTimeStampToDateTimeInUTC(utcExpireDate);
                if (expireDate > DateTime.UtcNow) 
                    throw new Exception("Token has not expired yet!");

                // checks if token exists in database
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == tokenReq.RefreshToken);
                if (dbRefreshToken != null)
                    throw new Exception("Refresh token does not exists in database");
                else
                {
                    // checks validate id
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    if (dbRefreshToken.JwtId != jti)
                        throw new Exception("Token does not match");

                    // checks refresh token expiration
                    if (dbRefreshToken.DateExpire <= DateTime.UtcNow)
                        throw new Exception("Your refresh token has expired, please re-authenticate");

                    // checks if refresh token is revoked
                    if (dbRefreshToken.IsRevoked)
                        throw new Exception("Refresh token is revoked");

                    // generates new token with existing refresh token
                    var dbUser = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                    var newTokenResponse = GenerateJwtTokenAsync(dbUser, tokenReq.RefreshToken);

                    return await newTokenResponse;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == tokenReq.RefreshToken);

                // generates new token with existing refresh token
                var dbUser = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                var newTokenResponse = GenerateJwtTokenAsync(dbUser, tokenReq.RefreshToken);

                return await newTokenResponse;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            
        }
        private DateTime UnixTimeStampToDateTimeInUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }
    }
}
