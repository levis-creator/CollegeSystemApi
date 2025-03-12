
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CollegeSystemApi.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context =  serviceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppUserRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        string[] roleNames = { "Admin", "Student", "Instructor" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppUserRole
                {
                    Name=roleName
                });
            }
        }

        var adminEmail = "admin@email.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "AdminPassword123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
