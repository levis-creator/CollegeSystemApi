using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class AcademicYear:BaseEntity
{
    public string AcademicPeriod { get; set; } = string.Empty;
}
