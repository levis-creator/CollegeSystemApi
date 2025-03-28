using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.DTOs;

public interface IStudentCrudService
{
    Task<ResponseDto<StudentDto>> CreateStudentAsync(StudentCreateDto studentDto);
    Task<ResponseDto<StudentDto>> GetStudentByIdAsync(string id);
    Task<ResponseDto<StudentDto>> GetAllStudentsAsync();
    Task<ResponseDto<StudentDto>> UpdateStudentAsync(string id, StudentUpdateDto studentDto);
    Task<ResponseDto> DeleteStudentAsync(string id);
}
