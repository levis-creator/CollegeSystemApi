using CollegeSystemApi.Data;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;

namespace CollegeSystemApi.Services;

public class AcademicYearService : GenericServices<AcademicYear>, IAcademicYearService
{
    public AcademicYearService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
    {
    }
}
