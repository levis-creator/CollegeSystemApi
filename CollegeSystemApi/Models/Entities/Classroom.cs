using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class Classroom:BaseEntity
{
    public string ClassName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
}
