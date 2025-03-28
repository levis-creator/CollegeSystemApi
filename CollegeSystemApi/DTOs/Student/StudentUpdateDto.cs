using System.ComponentModel.DataAnnotations;

namespace CollegeSystemApi.DTOs.Student;

public class StudentUpdateDto
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public int NationalId { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public int DepartmentId { get; set; }


}
