using CollegeSystemApi.Data;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;

namespace CollegeSystemApi.Services;

public class CourseUnitService : GenericServices<CourseUnit>, ICourseUnitService
{
    public CourseUnitService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
    {
    }
}
