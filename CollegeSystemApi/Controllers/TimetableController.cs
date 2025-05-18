using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Timetable;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimetableController(
    ITimetableService timetableService,
    ILogger<TimetableController> logger) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDtoData<TimetableDto>>> CreateTimetable([FromBody] CreateTimetableDto dto)
    {
        logger.LogInformation("Creating timetable");
        var result = await timetableService.CreateTimetableAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDtoData<TimetableDto>>> GetTimetableById(int id)
    {
        logger.LogInformation("Fetching timetable with ID: {Id}", id);
        var result = await timetableService.GetTimetableByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDtoData<List<TimetableDto>>>> GetAllTimetables()
    {
        logger.LogInformation("Fetching all timetables");
        var result = await timetableService.GetAllTimetablesAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}/with-schedules")]
    public async Task<ActionResult<ResponseDtoData<TimetableWithSchedulesDto>>> GetTimetableWithSchedules(int id)
    {
        logger.LogInformation("Fetching timetable with schedules for ID: {Id}", id);
        var result = await timetableService.GetTimetableWithSchedulesAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDtoData<TimetableDto>>> UpdateTimetable(int id, [FromBody] UpdateTimetableDto dto)
    {
        logger.LogInformation("Updating timetable with ID: {Id}", id);
        var result = await timetableService.UpdateTimetableAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteTimetable(int id)
    {
        logger.LogInformation("Deleting timetable with ID: {Id}", id);
        var result = await timetableService.DeleteTimetableAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
