using CollegeSystemApi.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeSystemApi.Models.Entities;

public class Student:BaseEntity
{
    public int NationalId { get; set; }
    public string UserId { get; set; }= string.Empty;
    public AppUser User { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string AdmNo { get; set; } = string.Empty;
  
    public int? ProgrammeId { get; set; }
    public Programme? Programme { get; set; }
    public DateOnly AdmissionDate { get; set; }
    public bool IsActive { get; set; } = true;
}
