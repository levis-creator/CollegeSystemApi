using CollegeSystemApi.Models.Common;
using Microsoft.AspNetCore.Identity;

namespace CollegeSystemApi.Models;

public class AppUser: IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

}
