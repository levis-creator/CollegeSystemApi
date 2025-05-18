namespace CollegeSystemApi.DTOs.Schedule;

public class ScheduleDto
{
    public int Id { get; set; }
    public string Day { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public int CourseUnitId { get; set; }
    public string CourseUnitName { get; set; } = string.Empty;
    public string CourseUnitCode { get; set; } = string.Empty;

    public int ClassroomId { get; set; }
    public string ClassroomName { get; set; } = string.Empty;
    public string ClassroomShortName { get; set; } = string.Empty;
}
