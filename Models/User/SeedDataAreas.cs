using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.User
{
    public static class SeedDataAreas
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<LocalUser>>();

            string[] roleNames = { "User", "Admin" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    Console.WriteLine($"Creating role: {roleName}");
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed admin user (your existing code)...
        }
    }
}
