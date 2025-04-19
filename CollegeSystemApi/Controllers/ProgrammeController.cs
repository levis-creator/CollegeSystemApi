using CollegeSystemApi.DTOs.Programme;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Services.Interfaces.IProgrammeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProgrammeController(
    IProgrammeService programmeService,
    ILogger<ProgrammeController> logger) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDtoData<ProgrammeDto>>> CreateProgramme([FromBody] CreateProgrammeDto programmeDto)
    {
        logger.LogInformation("Creating programme");
        var result = await programmeService.CreateProgrammeAsync(programmeDto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDtoData<ProgrammeDto>>> GetProgrammeById(int id)
    {
        logger.LogInformation("Fetching programme with ID: {Id}", id);
        var result = await programmeService.GetProgrammeByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDtoData<List<ProgrammeDto>>>> GetAllProgrammes()
    {
        logger.LogInformation("Fetching all programmes");
        var result = await programmeService.GetAllProgrammesAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("list")]
    public async Task<ActionResult<ResponseDtoData<List<ProgrammeListDto>>>> GetAllProgrammeList()
    {
        logger.LogInformation("Fetching simplified programme list");
        var result = await programmeService.GetAllProgrammeListAsync();
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDtoData<ProgrammeDto>>> UpdateProgramme(int id, [FromBody] UpdateProgrammeDto programmeDto)
    {
        logger.LogInformation("Updating programme with ID: {Id}", id);
        var result = await programmeService.UpdateProgrammeAsync(id, programmeDto);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteProgramme(int id)
    {
        logger.LogInformation("Deleting programme with ID: {Id}", id);
        var result = await programmeService.DeleteProgrammeAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
