﻿namespace CollegeSystemApi.DTOs.Auth
{
    public class AssignRoleRequest
    {
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
