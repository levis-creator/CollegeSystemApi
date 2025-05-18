using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities
{
    public class TimeTable:BaseEntity
    {
        public AcademicYear? AcademicYear { get; set; }
        public IEnumerable<Schedule> Schedules { get; set; } = [];
    }
}
