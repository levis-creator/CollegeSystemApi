using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.DTOs;

namespace CollegeSystemApi.Services.Interfaces.StudentServices
{
    // IStudentOperationsService.cs
    public interface IStudentOperationsService
    {
        Task<ResponseDto> ChangeStudentDepartmentAsync(string studentId, int newDepartmentId);
        Task<ResponseDto> DeactivateStudentAsync(string id);
        Task<ResponseDto<StudentDto>> GetStudentsByDepartmentAsync(int departmentId);
        Task<ResponseDto> ReactivateStudentAsync(string id);
    }
}
