using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.User;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<ResponseDtoData<List<UserAdminDisplayDto>>> GetAllUsersAsync();
        Task<ResponseDtoData<UserAdminDisplayDto>> GetUserByIdAsync(string userId);
    }
}
