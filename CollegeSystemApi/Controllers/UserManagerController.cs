using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagementService userManagement;

        public UserManagerController(IUserManagementService userManagement)
        {
            this.userManagement = userManagement;
        }
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var results = await userManagement.GetAllUsersAsync();
            return Ok(results);
        }
        [HttpGet("User/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var results = await userManagement.GetUserByIdAsync(id);
            return results.StatusCode == 200 ? Ok(results) : results.StatusCode == 404 ? NotFound(results) : BadRequest(results);
        }

    }
}
