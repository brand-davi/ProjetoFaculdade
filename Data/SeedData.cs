using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Data
{
    public static class SeedData
    {
        private static readonly string[] Roles = new[] { "Admin", "Manager", "Collaborator" };

        public static async Task InitializeAsync(IServiceProvider sp, string adminEmail, string adminPassword)
        {
            using var scope = sp.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    CPF = "00000000000",
                    Position = "Administrator"
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
                    throw new Exception("Failed to create admin user: " + errors);
                }
            }
        }
    }
}
