using CollegeSystemApi.Data;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;

namespace CollegeSystemApi.Services
{
    public class ClassroomService : GenericServices<Classroom>, IClassroomService
    {
        public ClassroomService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
    }
}
