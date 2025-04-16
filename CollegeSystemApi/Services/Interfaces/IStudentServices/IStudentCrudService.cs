using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Student;

namespace CollegeSystemApi.Services.Interfaces.IStudentServices;

public interface IStudentCrudService
{
    Task<ResponseDtoData<StudentDto>> CreateStudentAsync(StudentCreateDto studentDto);
    Task<ResponseDtoData<StudentDto>> GetStudentByIdAsync(int id);
    Task<ResponseDtoData<List<StudentDto>>> GetActiveStudents();
    Task<ResponseDtoData<List<StudentDto>>> GetAllStudentsAsync();
    Task<ResponseDtoData<StudentDto>> UpdateStudentAsync(int id, StudentUpdateDto studentDto);
    Task<ResponseDto> DeleteStudentAsync(int id);
}