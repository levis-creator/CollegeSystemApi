﻿using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models;

public class Student:BaseEntity
{
    public int NationalId { get; set; }
    public string UserId { get; set; }= string.Empty;
    public AppUser User { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string AdmNo { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
