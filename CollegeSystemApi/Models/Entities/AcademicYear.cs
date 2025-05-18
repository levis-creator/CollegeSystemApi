using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class AcademicYear:BaseEntity
{
    public string AcademicPeriod { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Year { get; set; }
}
