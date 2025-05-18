using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class CourseUnit : BaseEntity
{
    public string UnitName { get; set; } = string.Empty;
    public string UnitCode { get; set; }= string.Empty;
    public int Credits { get; set; }
}
