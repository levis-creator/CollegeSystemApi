using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.DTOs.User;
using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CollegeSystemApi.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppUserRole> _roleManager;

        public UserManagementService(UserManager<AppUser> userManager, RoleManager<AppUserRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<UserAdminDisplayDto>> GetAllUsersAsync()
        {
            // Retrieve all users from the database
            var dbUsers = _userManager.Users.ToList();

            // Map the users to UserAdminDisplayDto
            var userDtos = new List<UserAdminDisplayDto>();

            foreach (var user in dbUsers)
            {
                // Get the roles for the current user
                var roles = await _userManager.GetRolesAsync(user);

                // Map the user to UserAdminDisplayDto
                var userDto = new UserAdminDisplayDto
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? string.Empty,  // Null check for email
                    PhoneNumber = user.PhoneNumber ?? string.Empty,  // Null check for phone number
                    Role = roles.Any() ? string.Join(", ", roles) : "No roles assigned" // Handle multiple roles and default message if none
                };

                userDtos.Add(userDto);
            }

            // Return success response with list of users
            return ResponseDto<UserAdminDisplayDto>.SuccessResultForList(userDtos, "Users retrieved successfully");
        }

        public async Task<ResponseDto<UserAdminDisplayDto>> GetUserByIdAsync(string userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Return error response when user is not found
                return ResponseDto<UserAdminDisplayDto>.ErrorResultForData((int)HttpStatusCode.NotFound, "User not found");
            }

            // Get the roles for the user
            var roles = await _userManager.GetRolesAsync(user);

            // Map the user to UserAdminDisplayDto
            var userDto = new UserAdminDisplayDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email ?? string.Empty,  // Null check for email
                PhoneNumber = user.PhoneNumber ?? string.Empty,  // Null check for phone number
                Role = roles.Any() ? string.Join(", ", roles) : "No roles assigned" // Handle multiple roles and default message if none
            };

            // Return success response for the individual user
            return ResponseDto<UserAdminDisplayDto>.SuccessResultForData(userDto, "User retrieved successfully");
        }
    }
}
