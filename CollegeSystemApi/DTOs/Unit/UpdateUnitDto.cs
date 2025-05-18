namespace CollegeSystemApi.DTOs.Courses;

public class UpdateUnitDto
{
    public string UnitName { get; set; } = string.Empty;
    public string UnitCode { get; set; } = string.Empty;
    public int Credits { get; set; }
}