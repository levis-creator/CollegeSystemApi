namespace CollegeSystemApi.DTOs.Student;

public class StudentDto
{
    public string? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string AdmNo { get; set; } = string.Empty;
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string ProgrammeId { get; set; } = string.Empty;
    public string ProgrammeCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public string? DepartmentCode { get; set; }
}