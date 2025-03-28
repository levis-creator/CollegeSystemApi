//using CollegeSystemApi.Data;
//using CollegeSystemApi.DTOs.Student;
//using CollegeSystemApi.DTOs;
//using CollegeSystemApi.Models;
//using Microsoft.AspNetCore.Identity;
//using System.Net;
//using Microsoft.EntityFrameworkCore;

//public class StudentCrudService : IStudentCrudService
//{
//    private readonly UserManager<AppUser> _userManager;
//    private readonly ApplicationDbContext _context;

//    public StudentCrudService(
//        UserManager<AppUser> userManager,
//        ApplicationDbContext context)
//    {
//        _userManager = userManager;
//        _context = context;
//    }

//    public async Task<ResponseDto<StudentDto>> CreateStudentAsync(StudentCreateDto studentDto)
//    {
//        try
//        {
//            // Validate department exists
//            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId);
//            if (!departmentExists)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    "Specified department does not exist");
//            }

//            // Check for duplicate admission number
//            var studentExistByAdm = await _context.Students.FirstOrDefaultAsync(s => s.AdmNo == studentDto.AdmNo);
//            if (studentExistByAdm != null)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.Conflict,
//                    "Student with this admission number already exists");
//            }

//            // Check for duplicate email
//            var userExist = await _userManager.FindByEmailAsync(studentDto.Email);
//            if (userExist != null)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.Conflict,
//                    "User with this email already exists");
//            }

//            // Create new student
//            var student = new Student
//            {
//                FirstName = studentDto.FirstName,
//                LastName = studentDto.LastName,
//                Email = studentDto.Email,
//                UserName = studentDto.Email,
//                AdmNo = studentDto.AdmNo,
//                NationalId = int.Parse(studentDto.NationalId),
//                DepartmentId = studentDto.DepartmentId
//            };

//            // Use National ID as password
//            var result = await _userManager.CreateAsync(student, studentDto.NationalId);
//            if (!result.Succeeded)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//            }

//            // Add student to Student role
//            var roleResult = await _userManager.AddToRoleAsync(student, "Student");
//            if (!roleResult.Succeeded)
//            {
//                await _userManager.DeleteAsync(student);
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.InternalServerError,
//                    "Failed to add student to role");
//            }

//            // Return created student
//            var createdStudent = await _context.Students
//                .Include(s => s.Department)
//                .FirstOrDefaultAsync(s => s.Id == student.Id);

//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.SuccessResult(
//                MapToStudentDto(createdStudent!),
//                "Student created successfully",
//                (int)HttpStatusCode.Created);
//        }
//        catch (Exception ex)
//        {
//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"An error occurred while creating student: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto> DeleteStudentAsync(string id)
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

//            var result = await _userManager.DeleteAsync(student);
//            if (!result.Succeeded)
//            {
//                return ResponseDto.ErrorResult(
//                    (int)HttpStatusCode.InternalServerError,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//            }

//            return ResponseDto.SuccessResult("Student deleted successfully");
//        }
//        catch (Exception ex)
//        {
//            return ResponseDto.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"An error occurred while deleting student: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto<StudentDto>> GetAllStudentsAsync()
//    {
//        try
//        {
//            var students = await _context.Students
//                .Include(s => s.Department)
//                .ToListAsync();

//            return ResponseDto<StudentDto>.SuccessResultForList(
//                students.Select(MapToStudentDto).ToList(),
//                "Students retrieved successfully");
//        }
//        catch (Exception ex)
//        {
//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult((int)HttpStatusCode.InternalServerError, $"An error occurred while retrieving students: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto<StudentDto>> GetStudentByIdAsync(string id)
//    {
//        try
//        {
//            var student = await _context.Students
//                .Include(s => s.Department)
//                .FirstOrDefaultAsync(s => s.Id == id);

//            if (student == null)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.NotFound,
//                    "Student not found");
//            }

//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.SuccessResult(
//                MapToStudentDto(student),
//                "Student retrieved successfully");
//        }
//        catch (Exception ex)
//        {
//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"An error occurred while retrieving student: {ex.Message}");
//        }
//    }

//    public async Task<ResponseDto<StudentDto>> UpdateStudentAsync(string id, StudentUpdateDto studentDto)
//    {
//        try
//        {
//            var student = await _context.Students
//                .Include(s => s.Department)
//                .FirstOrDefaultAsync(s => s.Id == id);

//            if (student == null)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.NotFound,
//                    "Student not found");
//            }

//            // Update required fields
//            student.FirstName = studentDto.FirstName;
//            student.LastName = studentDto.LastName;
//            student.NationalId = studentDto.NationalId;

//            // Update department if different
//            if (studentDto.DepartmentId != 0 && studentDto.DepartmentId != student.DepartmentId)
//            {
//                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId);
//                if (!departmentExists)
//                {
//                    return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                        (int)HttpStatusCode.BadRequest,
//                        "Specified department does not exist");
//                }
//                student.DepartmentId = studentDto.DepartmentId;
//            }

//            // Update email if provided and different
//            if (!string.IsNullOrEmpty(studentDto.Email) && studentDto.Email != student.Email)
//            {
//                var emailExists = await _userManager.FindByEmailAsync(studentDto.Email);
//                if (emailExists != null && emailExists.Id != student.Id)
//                {
//                    return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                        (int)HttpStatusCode.Conflict,
//                        "Email already in use by another user");
//                }
//                student.Email = studentDto.Email;
//                student.UserName = studentDto.Email;
//                student.NormalizedEmail = studentDto.Email.ToUpper();
//                student.NormalizedUserName = studentDto.Email.ToUpper();
//            }

//            var result = await _userManager.UpdateAsync(student);
//            if (!result.Succeeded)
//            {
//                return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                    (int)HttpStatusCode.BadRequest,
//                    string.Join(", ", result.Errors.Select(e => e.Description)));
//            }

//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.SuccessResult(
//                MapToStudentDto(student),
//                "Student updated successfully");
//        }
//        catch (Exception ex)
//        {
//            return (ResponseDto<StudentDto>)ResponseDto<StudentDto>.ErrorResult(
//                (int)HttpStatusCode.InternalServerError,
//                $"An error occurred while updating student: {ex.Message}");
//        }
//    }

//    private StudentDto MapToStudentDto(Student student)
//    {
//        return new StudentDto
//        {
//            Id = student.Id,
//            FirstName = student.FirstName,
//            LastName = student.LastName,
//            Email = student.Email,
//            NationalId = student.NationalId.ToString(),
//            AdmNo = student.AdmNo,
//            DepartmentId = student.DepartmentId,
//            DepartmentName = student.Department?.DepartmentName
//        };
//    }
//}
