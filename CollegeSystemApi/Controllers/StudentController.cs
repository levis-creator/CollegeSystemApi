using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.Services.Interfaces.IStudentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeSystemApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController(
    IStudentCrudService crudService,
    // IStudentOperationsService operationsService,
    ILogger<StudentsController> logger)
    : ControllerBase
{
    // POST: api/students
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseDtoData<StudentDto>>> CreateStudent([FromBody] StudentCreateDto studentDto)
    {
        logger.LogInformation("Creating new student");
        var result = await crudService.CreateStudentAsync(studentDto);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/students/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDto<StudentDto>>> GetStudentById(int id)
    {
        logger.LogInformation("Getting student with ID: {Id}", id);
        var result = await crudService.GetStudentByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // GET: api/students
    [HttpGet]
    public async Task<ActionResult<ResponseDtoData<List<StudentDto>>>> GetStudents()
    {
        logger.LogInformation("Getting all students");
        var result = await crudService.GetActiveStudents();
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("/all")]
    public async Task<ActionResult<ResponseDtoData<List<StudentDto>>>> GetAllStudent()
    {
        logger.LogInformation("Getting all students");
        var result = await crudService.GetAllStudentsAsync();
        return StatusCode(result.StatusCode, result);
    }

    // PUT: api/students/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDtoData<StudentDto>>> UpdateStudent(
        int id,
        [FromBody] StudentUpdateDto studentDto)
    {
        logger.LogInformation("Updating student with ID: {Id}", id);
        var result = await crudService.UpdateStudentAsync(id, studentDto);
        return StatusCode(result.StatusCode, result);
    }

    // DELETE: api/students/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> DeleteStudent(int id)
    {
        logger.LogInformation("Deleting student with ID: {Id}", id);
        var result = await crudService.DeleteStudentAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // // PATCH: api/students/{studentId}/department/{departmentId}
    // [Authorize(Roles = "Admin")]
    // [HttpPatch("{studentId}/department/{departmentId}")]
    // public async Task<ActionResult<ResponseDto>> ChangeStudentDepartment(
    //     string studentId,
    //     int departmentId)
    // {
    //     logger.LogInformation("Changing department for student {StudentId} to {DepartmentId}",
    //         studentId, departmentId);
    //     var result = await operationsService.ChangeStudentDepartmentAsync(studentId, departmentId);
    //     return StatusCode(result.StatusCode, result);
    // }
    //
    // // GET: api/students/department/{departmentId}
    // [HttpGet("department/{departmentId}")]
    // public async Task<ActionResult<ResponseDto<IEnumerable<StudentDto>>>> GetStudentsByDepartment(
    //     int departmentId)
    // {
    //     logger.LogInformation("Getting students in department: {DepartmentId}", departmentId);
    //     var result = await operationsService.GetStudentsByDepartmentAsync(departmentId);
    //     return StatusCode(result.StatusCode, result);
    // }
}