namespace CollegeSystemApi.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; } // optional, for display
}