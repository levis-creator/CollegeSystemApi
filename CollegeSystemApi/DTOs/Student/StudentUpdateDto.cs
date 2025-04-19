using System.ComponentModel.DataAnnotations;

namespace CollegeSystemApi.DTOs.Student;

public class StudentUpdateDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string NationalId { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; }= string.Empty;

    public int DepartmentId { get; set; }
    public int ProgrammeId { get; set; }
    public string? AdmNo { get; set; }
}
