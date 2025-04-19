using CollegeSystemApi.Models.Enum;

namespace CollegeSystemApi.DTOs.Programme;

public class ProgrammeListDto
{
    public int Id { get; set; }
    public string ProgrammeName { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // Output as string
    public string DepartmentName { get; set; } = string.Empty;
}
