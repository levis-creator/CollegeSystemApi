﻿using System.Text;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Auth;
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services;
using CollegeSystemApi.Services.CoursesServices;
using CollegeSystemApi.Services.Interfaces;
using CollegeSystemApi.Services.Interfaces.ICoursesServices;
using CollegeSystemApi.Services.Interfaces.IProgrammeServices;
using CollegeSystemApi.Services.Interfaces.IStudentServices;
using CollegeSystemApi.Services.ProgrammeServices;
using CollegeSystemApi.Services.StudentServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollegeSystemApi.Extensions;

public static class ServiceExtension
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
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericServices<>), typeof(GenericServices<>));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IStudentCrudService, StudentCrudService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseUnitService, CourseUnitService>();
        services.AddScoped<IProgrammeService, ProgrammeService>();
        services.AddScoped<IAcademicYearService, AcademicYearService>();
        services.AddScoped<IClassroomService, ClassroomService>();
        services.AddScoped<ITimetableService, TimetableService>();
        
        //services.AddScoped<IStudentOperationsService, StudentOperationsService>();

        return services;
    }
  
    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });
        return services;
    }

}
