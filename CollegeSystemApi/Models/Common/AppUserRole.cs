using Microsoft.AspNetCore.Identity;

namespace CollegeSystemApi.Models.Common;

public class AppUserRole:IdentityRole
{
    public string? Description { get; set; } = null;
    public string? CreatedBy { get; internal set; }
}
