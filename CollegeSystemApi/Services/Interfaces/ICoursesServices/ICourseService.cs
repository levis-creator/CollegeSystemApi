using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.DTOs.Response;

namespace CollegeSystemApi.Services.Interfaces.ICoursesServices;

public interface ICourseService
{
    Task<ResponseDtoData<CourseDto>> CreateCourseAsync(CreateCourseDto courseDto);
    Task<ResponseDtoData<CourseDto>> GetCourseByIdAsync(int id);
    Task<ResponseDtoData<List<CourseDto>>> GetAllCoursesAsync();
    Task<ResponseDtoData<List<CourseListDto>>> GetAllCourseListAsync();
    Task<ResponseDtoData<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto courseDto);
    Task<ResponseDto> DeleteCourseAsync(int id);
}
