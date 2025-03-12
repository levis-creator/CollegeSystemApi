using CollegeSystemApi.DTOs.Auth;
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services.Interfaces;
using CollegeSystemApi.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace CollegeSystemApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppUserRole> _roleManager;
    private readonly JwtSettings _jwtSettings;
    public AuthService(UserManager<AppUser> userManager,
        RoleManager<AppUserRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
    }

    public async Task<AuthResponse> AddUserToRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new AuthResponse(false, "User not found");
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new AuthResponse(false, "Role not found");
        var result = await _userManager.AddToRoleAsync(user, roleName);

        return result.Succeeded
           ? new AuthResponse(true, $"User added to {roleName} role")
           : new AuthResponse(false, GetErrors(result.Errors));
    }

    public async Task<AuthResponse> CreateRoleAsync(RoleRequestDto request, string currentUserId)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new AuthResponse(false, "Role name is required");
        if (await _roleManager.RoleExistsAsync(request.Name))
            return new AuthResponse(false, "Role already exist");
        var result = await _roleManager.CreateAsync(new AppUserRole
        {
            Name = request.Name,
            CreatedBy = currentUserId
        });
        return result.Succeeded
            ? new AuthResponse(true, "Role created successfully")
            : new AuthResponse(false, GetErrors(result.Errors));
    }

    public async Task<AuthResponse> LoginUserAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return new AuthResponse(false, "Invalid credentials");
        var userRoles = await _userManager.GetRolesAsync(user);
        var token = await TokenHelper.GenerateToken(
            user,
            _userManager,
            _jwtSettings
            );
        return new AuthResponse(
            success: true,
            message: "Authentication Successful",
            token: token,
            fullName: $"{user.FirstName} {user.LastName}",
            email: user.Email,
            roles: userRoles);
    }

    public async Task<AuthResponse> RegisterUserAsync(RegisterRequestDto request)
    {
        var existUser = await _userManager.FindByEmailAsync(request.Email);
        if (existUser != null)
            return new AuthResponse(false, "User already exists");
        AppUser newUser = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };
        var createResults = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResults.Succeeded)
            return new AuthResponse(false, GetErrors(createResults.Errors));
        await _userManager.AddToRoleAsync(newUser, "Student");

        return new AuthResponse(true, "User created successfully");
    }
    private static string GetErrors(IEnumerable<IdentityError> errors)
    => string.Join(", ", errors.Select(e => e.Description));
}
