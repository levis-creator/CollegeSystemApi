using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, AppUserRole, string>(options)
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Student Entity Relationship
        builder.Entity<Student>()
            .HasIndex(s => s.NationalId)
            .IsUnique();
            
        builder.Entity<Student>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.Entity<Student>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId);
    }
}
