using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.User;

public static class SeedData
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
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed admin user
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            adminUser = new LocalUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                FirstName = "abdullah",
                LastName = "mohamden",
                Phone = 01234567890,
                Address = "123 Admin Street, Cairo",
                SSN = "12345678901234",
                EmailConfirmed = true // Pre-confirmed for admin
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}