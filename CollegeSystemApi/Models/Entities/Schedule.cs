using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class Schedule:BaseEntity
{
    public DayOfWeek Day { get; set; }
    public int ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
    public int CourseUnitId { get; set; }
    public CourseUnit? CourseUnit { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
