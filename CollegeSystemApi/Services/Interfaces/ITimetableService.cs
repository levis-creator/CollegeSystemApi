using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Timetable;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface ITimetableService
    {
        Task<ResponseDtoData<TimetableDto>> CreateTimetableAsync(CreateTimetableDto dto);
        Task<ResponseDtoData<TimetableDto>> GetTimetableByIdAsync(int id);
        Task<ResponseDtoData<List<TimetableDto>>> GetAllTimetablesAsync();
        Task<ResponseDtoData<TimetableDto>> UpdateTimetableAsync(int id, UpdateTimetableDto dto);
        Task<ResponseDto> DeleteTimetableAsync(int id);
        Task<ResponseDtoData<TimetableWithSchedulesDto>> GetTimetableWithSchedulesAsync(int id);
    }
}
