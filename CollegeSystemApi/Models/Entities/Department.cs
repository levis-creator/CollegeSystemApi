﻿using CollegeSystemApi.Models.Common;

namespace CollegeSystemApi.Models.Entities
{
    public class Department:BaseEntity
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Student> Students { get; set; } = [];
    }
}
