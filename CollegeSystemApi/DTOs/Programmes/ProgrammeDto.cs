namespace CollegeSystemApi.DTOs.Programme;

public class ProgrammeDto
{
    public int Id { get; set; }
    public string ProgrammeName { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // Output as string
    public int ProgrammeDuration { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
