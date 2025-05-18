using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Schedule;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ResponseDtoData<ScheduleDto>> CreateScheduleAsync(CreateScheduleDto dto);
        Task<ResponseDtoData<ScheduleDto>> GetScheduleByIdAsync(int id);
        Task<ResponseDtoData<List<ScheduleDto>>> GetAllSchedulesAsync();
        Task<ResponseDtoData<ScheduleDto>> UpdateScheduleAsync(int id, UpdateScheduleDto dto);
        Task<ResponseDto> DeleteScheduleAsync(int id);
    }
}
