using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities;

public class Course : BaseEntity
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; }= string.Empty;
    public int Credits { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
}
