namespace CollegeSystemApi.DTOs.Schedule
{
    public class CreateScheduleDto
    {
        public string Day { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int CourseUnitId { get; set; }
        public int ClassroomId { get; set; }
    }
}
