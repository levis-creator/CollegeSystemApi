using CollegeSystemApi.DTOs.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterUserAsync(DTOs.Auth.RegisterRequestDto request);
        Task<AuthResponse> LoginUserAsync(DTOs.Auth.LoginRequestDto request);
        Task<AuthResponse> CreateRoleAsync(RoleRequestDto request, string currentUserId);
        Task<AuthResponse> AddUserToRoleAsync(string email, string roleName);
        Task<AuthResponse> VerifyTokenAsync(string token);
        Task<CurrentUserDto?> GetLoggedOnUserAsync(string userId);
    }
}
