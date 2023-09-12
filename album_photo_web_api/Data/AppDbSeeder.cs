using album_photo_web_api.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace album_photo_web_api.Data
{
    public class AppDbSeeder
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();

                context.SaveChanges();
            }
        }
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //Admins
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var adminUser = await userManager.FindByNameAsync("admin");
                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "admin",
                    };
                    await userManager.CreateAsync(newAdminUser, "Test123!");
                    await userManager.AddClaimAsync(newAdminUser, new Claim(ClaimTypes.Role, "ADMIN"));
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                //Users

                var user = await userManager.FindByNameAsync("user");
                if (user == null)
                {
                    var newUser = new User()
                    {
                        UserName = "user"
                    };
                    await userManager.CreateAsync(newUser, "Test123!");
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);
                }
            }
        }
    }
}

