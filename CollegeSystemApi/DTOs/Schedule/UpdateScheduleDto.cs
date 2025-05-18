namespace CollegeSystemApi.DTOs.Schedule
{
    public class UpdateScheduleDto
    {
        public string? Day { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int? CourseUnitId { get; set; }
        public int? ClassroomId { get; set; }
    }
}
