using album_photo_web_api.Data;
using album_photo_web_api.Data.ViewModels.Authentication;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AuthenticationController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
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

        private async Task<AuthenticationResultVM> GenerateJwtToken(User user)
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

            var refreshToken = new RefreshToken()
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

            var response = new AuthenticationResultVM()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
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
                var tokenValue = await GenerateJwtToken(user);
                return Ok(tokenValue);
            }

            return Unauthorized();
        }
    }
}
