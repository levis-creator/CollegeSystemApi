using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Schedule;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController(
    IScheduleService scheduleService,
    ILogger<ScheduleController> logger) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDtoData<ScheduleDto>>> CreateSchedule([FromBody] CreateScheduleDto dto)
    {
        logger.LogInformation("Creating a new schedule");
        var result = await scheduleService.CreateScheduleAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDtoData<ScheduleDto>>> GetScheduleById(int id)
    {
        logger.LogInformation("Getting schedule with ID: {Id}", id);
        var result = await scheduleService.GetScheduleByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDtoData<List<ScheduleDto>>>> GetAllSchedules()
    {
        logger.LogInformation("Getting all schedules");
        var result = await scheduleService.GetAllSchedulesAsync();
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDtoData<ScheduleDto>>> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
    {
        logger.LogInformation("Updating schedule with ID: {Id}", id);
        var result = await scheduleService.UpdateScheduleAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteSchedule(int id)
    {
        logger.LogInformation("Deleting schedule with ID: {Id}", id);
        var result = await scheduleService.DeleteScheduleAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
