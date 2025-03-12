using Azure.Core;
using CollegeSystemApi.DTOs.Auth;
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IAuthService authService;
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto register)
    {
        var results = await authService.RegisterUserAsync(register);
        return results.Success ? Ok(results) : BadRequest(results);
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
    {
        var results = await authService.LoginUserAsync(login);
        return results.Success ? Ok(results) : Unauthorized(results);
    }
    [HttpPost("Role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRolesAsync([FromBody] RoleRequestDto role)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await authService.CreateRoleAsync(role, currentUserId!);
        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }
    [HttpPost("Assign-role")]
    [Authorize(Roles = "Admin")]
    public  async Task<IActionResult> AddUserToRoleAsync([FromBody]AssignRoleRequest request)
    {
        var result = await authService.AddUserToRoleAsync(request.Email, request.RoleName);
        return result.Success
        ? Ok(result)
            : BadRequest(result);
    }
}
