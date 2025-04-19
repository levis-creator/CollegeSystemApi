namespace CollegeSystemApi.DTOs.Programme;

public class UpdateProgrammeDto
{
    public string ProgrammeName { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty; // Input as string
    public int ProgrammeDuration { get; set; }
    public int DepartmentId { get; set; }
    public string Description { get; set; } = string.Empty;
}