using CollegeSystemApi.DTOs.Schedule;

namespace CollegeSystemApi.DTOs.Timetable
{
    public class TimetableWithSchedulesDto
    {
        public int Id { get; set; }
        public string AcademicPeriod { get; set; } = string.Empty;
        public List<ScheduleDto> Schedules { get; set; } = [];
    }
}
