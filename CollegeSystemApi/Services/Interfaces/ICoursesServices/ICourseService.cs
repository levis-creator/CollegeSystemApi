using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.DTOs.Response;

namespace CollegeSystemApi.Services.Interfaces.ICoursesServices;

public interface ICourseService
{
    Task<ResponseDtoData<UnitDto>> CreateCourseAsync(CreateUnitDto courseDto);
    Task<ResponseDtoData<UnitDto>> GetCourseByIdAsync(int id);
    Task<ResponseDtoData<List<UnitDto>>> GetAllCoursesAsync();
    Task<ResponseDtoData<List<UnitListDto>>> GetAllCourseListAsync();
    Task<ResponseDtoData<UnitDto>> UpdateCourseAsync(int id, UpdateUnitDto courseDto);
    Task<ResponseDto> DeleteCourseAsync(int id);
}
