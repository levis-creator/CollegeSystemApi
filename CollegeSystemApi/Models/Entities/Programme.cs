using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Models.Enum;

namespace CollegeSystemApi.Models.Entities;

public class Programme : BaseEntity
{
    public string ProgrammeName { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
    public LevelType Level { get; set; }
    public int ProgrammeDuration { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public List<Student> Students { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
