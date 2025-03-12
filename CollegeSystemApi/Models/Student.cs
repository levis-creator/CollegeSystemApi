using CollegeSystemApi.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace CollegeSystemApi.Models
{
    public class Student:BaseEntity
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        [Phone]
        public string Phone { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public int NationalId { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
