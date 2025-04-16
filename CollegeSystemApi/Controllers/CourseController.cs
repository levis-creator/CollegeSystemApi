using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Services.Interfaces.ICoursesServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(
    ICourseService courseService,
    ILogger<CourseController> logger) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDtoData<CourseDto>>> CreateCourse([FromBody] CreateCourseDto courseDto)
    {
        logger.LogInformation("Creating course");
        var result = await courseService.CreateCourseAsync(courseDto);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDtoData<CourseDto>>> GetCourseById(int id)
    {
        logger.LogInformation("Fetching course with ID: {Id}", id);
        var result = await courseService.GetCourseByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDtoData<List<CourseDto>>>> GetAllCourses()
    {
        logger.LogInformation("Fetching all courses");
        var result = await courseService.GetAllCoursesAsync();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("list")]
    public async Task<ActionResult<ResponseDtoData<List<CourseListDto>>>> GetAllCourseList()
    {
        logger.LogInformation("Fetching simplified course list");
        var result = await courseService.GetAllCourseListAsync();
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDtoData<CourseDto>>> UpdateCourse(int id, [FromBody] UpdateCourseDto courseDto)
    {
        logger.LogInformation("Updating course with ID: {Id}", id);
        var result = await courseService.UpdateCourseAsync(id, courseDto);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteCourse(int id)
    {
        logger.LogInformation("Deleting course with ID: {Id}", id);
        var result = await courseService.DeleteCourseAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
