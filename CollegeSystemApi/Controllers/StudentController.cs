
using CollegeSystemApi.DTOs;
using CollegeSystemApi.DTOs.Student;

using global::CollegeSystemApi.Services.Interfaces.StudentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentCrudService _crudService;
    private readonly IStudentOperationsService _operationsService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentCrudService crudService,
        IStudentOperationsService operationsService,
        ILogger<StudentsController> logger)
    {
        _crudService = crudService;
        _operationsService = operationsService;
        _logger = logger;
    }

    // POST: api/students
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDto<StudentDto>>> CreateStudent([FromBody] StudentCreateDto studentDto)
    {
        _logger.LogInformation("Creating new student");
        var result = await _crudService.CreateStudentAsync(studentDto);

        return result.StatusCode switch
        {
            (int)HttpStatusCode.Created => CreatedAtAction(
                nameof(GetStudentById),
                new { result.Data },
                result),
            _ => StatusCode(result.StatusCode, result)
        };
    }

    // GET: api/students/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto<StudentDto>>> GetStudentById(string id)
    {
        _logger.LogInformation("Getting student with ID: {Id}", id);
        var result = await _crudService.GetStudentByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // GET: api/students
    [HttpGet]
    public async Task<ActionResult<ResponseDto<IEnumerable<StudentDto>>>> GetAllStudents()
    {
        _logger.LogInformation("Getting all students");
        var result = await _crudService.GetAllStudentsAsync();
        return StatusCode(result.StatusCode, result);
    }

    // PUT: api/students/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDto<StudentDto>>> UpdateStudent(
        string id,
        [FromBody] StudentUpdateDto studentDto)
    {
        _logger.LogInformation("Updating student with ID: {Id}", id);
        var result = await _crudService.UpdateStudentAsync(id, studentDto);
        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/students/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteStudent(string id)
    {
        _logger.LogInformation("Deleting student with ID: {Id}", id);
        var result = await _crudService.DeleteStudentAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // PATCH: api/students/{studentId}/department/{departmentId}
    [Authorize(Roles = "Admin")]
    [HttpPatch("{studentId}/department/{departmentId}")]
    public async Task<ActionResult<ResponseDto>> ChangeStudentDepartment(
        string studentId,
        int departmentId)
    {
        _logger.LogInformation("Changing department for student {StudentId} to {DepartmentId}",
            studentId, departmentId);
        var result = await _operationsService.ChangeStudentDepartmentAsync(studentId, departmentId);
        return StatusCode(result.StatusCode, result);
    }

    // GET: api/students/department/{departmentId}
    [HttpGet("department/{departmentId}")]
    public async Task<ActionResult<ResponseDto<IEnumerable<StudentDto>>>> GetStudentsByDepartment(
        int departmentId)
    {
        _logger.LogInformation("Getting students in department: {DepartmentId}", departmentId);
        var result = await _operationsService.GetStudentsByDepartmentAsync(departmentId);
        return StatusCode(result.StatusCode, result);
    }
}

