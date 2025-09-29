using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WarrantyTracker.Models;

namespace WarrantyTracker.Data
{
    public static class RoleSeeder
    {
        private static readonly string[] Roles = new[] { "User", "Admin" };

        public static void Seed(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }

            SeedAdmin(serviceProvider); // ✅ call admin seeder
        }

        private static void SeedAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "deepvaishnav207@gmail.com";
            var admin = userManager.FindByEmailAsync(adminEmail).Result;

            if (admin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin"
                };

                var result = userManager.CreateAsync(adminUser, "Admin@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                }
            }
        }
    }
}
