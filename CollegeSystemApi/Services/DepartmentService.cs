using CollegeSystemApi.Data;
using CollegeSystemApi.Models;
using CollegeSystemApi.Services.Interfaces;


namespace CollegeSystemApi.Services
{
    public class DepartmentService : GenericServices<Department>, IDepartmentService
    {
        public DepartmentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        // Add any department-specific methods here.
    }
}
