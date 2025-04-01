using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.User;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<ResponseDto<UserAdminDisplayDto>> GetAllUsersAsync();
        Task<ResponseDto<UserAdminDisplayDto>> GetUserByIdAsync(string userId);
    }
}
