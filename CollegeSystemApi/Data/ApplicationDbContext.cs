using CollegeSystemApi.Models;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, AppUserRole, string>(options)
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Programme> Programmes { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Student Entity Relationship
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

        builder.Entity<Student>()
            .HasOne(s => s.Programme)
            .WithMany(p => p.Students)
            .HasForeignKey(s => s.ProgrammeId)
            .OnDelete(DeleteBehavior.NoAction);

        // Department Entity Relationship
        builder.Entity<Department>()
            .HasIndex(d => d.DepartmentCode)
            .IsUnique();

        // Course Entity Relationship
        builder.Entity<Course>()
            .HasIndex(c => c.CourseCode)
            .IsUnique();

        builder.Entity<Course>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Programme Entity Relationship
        builder.Entity<Programme>()
            .HasIndex(p => p.ProgrammeCode)
            .IsUnique();

        builder.Entity<Programme>()
            .HasOne(p => p.Department)
            .WithMany()
            .HasForeignKey(p => p.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
