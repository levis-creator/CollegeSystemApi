namespace CollegeSystemApi.DTOs.Courses;

public class CreateCourseDto
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int DepartmentId { get; set; }
}