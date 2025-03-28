//using CollegeSystemApi.Data;
//using CollegeSystemApi.DTOs.Student;
//using CollegeSystemApi.DTOs;
//using CollegeSystemApi.Models;
//using CollegeSystemApi.Services.Interfaces.StudentServices;
//using Microsoft.AspNetCore.Identity;
//using System.Net;
//using Microsoft.EntityFrameworkCore;

//namespace CollegeSystemApi.Services.StudentServices;

//// StudentOperationsService.cs
//public class StudentOperationsService : IStudentOperationsService
//{
//    private readonly UserManager<AppUser> _userManager;
//    private readonly ApplicationDbContext _context;
//    private readonly IStudentCrudService _studentCrudService;

//    public StudentOperationsService(
//        UserManager<AppUser> userManager,
//        ApplicationDbContext context,
//        IStudentCrudService studentCrudService)
//    {
//        _userManager = userManager;
//        _context = context;
//        _studentCrudService = studentCrudService;
//    }

//    public async Task<ResponseDto> ChangeStudentDepartmentAsync(string studentId, int newDepartmentId)
//    {
//        try
//        {
//            // First verify student exists
//            var studentResponse = await _studentCrudService.GetStudentByIdAsync(studentId);
//            if (!studentResponse.Success)
//            {
//                return ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.NotFound,
//                    "Student not found");
//            }

//            // Verify department exists
//            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == newDepartmentId);
//            if (!departmentExists)
//            {
//                return ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    "Department not found");
//            }

//            // Update department
//            var student = await _userManager.FindByIdAsync(studentId.ToString());
//            //student.DepartmentId = newDepartmentId;

//            var result = await _userManager.UpdateAsync(student);
//            return result.Succeeded
//                ? ResponseDto.SuccessResult("Department changed successfully")
//                : ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//        }
//        catch (Exception ex)
//        {
//            return ResponseDto.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"Error changing department: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto> DeactivateStudentAsync(string id)
//    {
//        try
//        {
//            var student = await _userManager.FindByIdAsync(id.ToString());
//            if (student == null)
//            {
//                return ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.NotFound,
//                    "Student not found");
//            }

//            student.Is = false;
//            var result = await _userManager.UpdateAsync(student);

//            return result.Succeeded
//                ? ResponseDto.SuccessResult("Student deactivated successfully")
//                : ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//        }
//        catch (Exception ex)
//        {
//            return ResponseDto.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"Error deactivating student: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto<StudentDto>> GetStudentsByDepartmentAsync(int departmentId)
//    {
//        try
//        {
//            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);
//            if (!departmentExists)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult((int)HttpStatusCode.BadRequest, "Department not found");
//            }

//            var students = await _context.Students
//                .Include(s => s.Department)
//                .Where(s => s.DepartmentId == departmentId && s.IsActive)
//                .ToListAsync();

//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.SuccessResult(
//                students.Select(s => new StudentDto
//                {
//                    Id = s.Id,
//                    FirstName = s.FirstName,
//                    LastName = s.LastName,
//                    Email = s.Email!,
//                    NationalId = s.NationalId.ToString(),
//                    AdmNo = s.AdmNo,
//                    DepartmentId = s.DepartmentId,
//                    DepartmentName = s.Department?.DepartmentName,
//                    IsActive = s.IsActive
//                }),
//                $"Students in department {departmentId} retrieved successfully");
//        }
//        catch (Exception ex)
//        {
//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"Error retrieving students by department: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto> ReactivateStudentAsync(string id)
//    {
//        try
//        {
//            var student = await _userManager.FindByIdAsync(id.ToString());
//            if (student == null)
//            {
//                return ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.NotFound,
//                    "Student not found");
//            }

//            student.IsActive = true;
//            var result = await _userManager.UpdateAsync(student);

//            return result.Succeeded
//                ? ResponseDto.SuccessResult("Student reactivated successfully")
//                : ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//        }
//        catch (Exception ex)
//        {
//            return ResponseDto.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"Error reactivating student: {ex.Message}");
//        }
//    }
//}