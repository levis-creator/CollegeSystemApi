
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var roleManager = services.GetRequiredService<RoleManager<AppUserRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                // Apply pending migrations
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully");

                // Seed roles
                await SeedRoles(roleManager, logger);

                // Seed admin user
                await SeedAdminUser(userManager, logger);

                // Seed departments (if using TPT, make sure to handle properly)
                await SeedDepartments(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static async Task SeedRoles(RoleManager<AppUserRole> roleManager, ILogger logger)
        {
            string[] roleNames = { "Admin", "Student", "Instructor" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation("Creating role {RoleName}", roleName);
                    var result = await roleManager.CreateAsync(new AppUserRole { Name = roleName });

                    if (!result.Succeeded)
                    {
                        logger.LogError("Failed to create role {RoleName}: {Errors}",
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<AppUser> userManager, ILogger logger)
        {
            const string adminEmail = "admin@email.com";
            const string adminPassword = "AdminPassword123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                logger.LogInformation("Creating admin user");

                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }

                result = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to add admin role to user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogInformation("Admin user created successfully");
                }
            }
        }

        private static async Task SeedDepartments(ApplicationDbContext context, ILogger logger)
        {
            if (!await context.Departments.AnyAsync())
            {
                logger.LogInformation("Seeding departments");

                var departments = new[]
                {
                    new Department { DepartmentName = "Computer Science", DepartmentCode = "CS" },
                    new Department { DepartmentName = "Electrical Engineering", DepartmentCode = "EE" },
                    new Department { DepartmentName = "Business Administration", DepartmentCode = "BA" }
                };

                try
                {
                    await context.Departments.AddRangeAsync(departments);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Departments seeded successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to seed departments");
                }
            }
        }
    }
}