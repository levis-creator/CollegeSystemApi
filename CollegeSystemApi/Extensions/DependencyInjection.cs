using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Auth;
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
namespace CollegeSystemApi.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddDataseConfiguration(this IServiceCollection services, string connectionString)
    {
        // Validate connection string parameter
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or whitespace", nameof(connectionString));
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Configure SQL Server with retry policy
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                }
            );

            // Optional: Add additional configurations here without changing method signature
#if DEBUG
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
        });
        return services;

    }
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind JWT settings from appsettings.json
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddIdentityApiEndpoints<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;



        })
            .AddRoles<AppUserRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            }
            );
        services.AddAuthorization();
        return services;
    }
    public static IServiceCollection AddServices(this IServiceCollection services) {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
