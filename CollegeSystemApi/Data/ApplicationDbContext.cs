using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Data;

public class ApplicationDbContext:IdentityDbContext<AppUser, AppUserRole, string> {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options) { }
}
