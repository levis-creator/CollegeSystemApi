namespace CollegeSystemApi.DTOs.Courses;

public class CreateUnitDto
{
    public string UnitName { get; set; } = string.Empty;
    public string UnitCode { get; set; } = string.Empty;
    public int Credits { get; set; }
}