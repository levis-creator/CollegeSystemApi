using AutoMapper;
using System.Net;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.Models;
using CollegeSystemApi.Services.Interfaces.IStudentServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using CollegeSystemApi.Models.Entities;

namespace CollegeSystemApi.Services.StudentServices
{
    public class StudentCrudService : IStudentCrudService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext context;
        private readonly ILogger<StudentCrudService> logger;
        private readonly IMapper mapper;

        public StudentCrudService(
            UserManager<AppUser> userManager,
            ApplicationDbContext context,
            ILogger<StudentCrudService> logger,
            IMapper mapper) // Inject AutoMapper
        {
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<ResponseDtoData<StudentDto>> CreateStudentAsync(StudentCreateDto studentDto)
        {
            try
            {
                var validationResult = await ValidateStudentCreateDto(studentDto);
                if (validationResult != null)
                    return validationResult;

                if (!int.TryParse(studentDto.NationalId, out int nationalId))
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid National ID format."
                    );
                }

                bool studentExists = await context.Students.AnyAsync(s => s.NationalId == nationalId);
                if (studentExists)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.Conflict,
                        "A student with this National ID already exists."
                    );
                }

                bool departmentExists = await context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId);
                if (!departmentExists)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Department ID."
                    );
                }

                var studentFirstName = studentDto.FirstName.Transform(To.TitleCase);
                var studentLastName = studentDto.LastName.Transform(To.TitleCase);
                var user = new AppUser
                {
                    FirstName = studentFirstName,
                    LastName = studentLastName,
                    Email = studentDto.Email,
                    UserName = studentDto.Email,
                    EmailConfirmed = true
                };
                var defaultPassword = $"{user.FirstName}{nationalId}#";

                var userResult = await userManager.CreateAsync(user, defaultPassword);
                if (!userResult.Succeeded)
                {
                    logger.LogError("User creation failed: {Errors}",
                        string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "User creation failed: " + string.Join(", ", userResult.Errors.Select(e => e.Description))
                    );
                }

                var roleResult = await userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Role assignment failed: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    await userManager.DeleteAsync(user);
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.InternalServerError,
                        "Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description))
                    );
                }

                var student = new Student
                {
                    NationalId = nationalId,
                    DepartmentId = studentDto.DepartmentId,
                    UserId = user.Id,
                    AdmNo = studentDto.AdmNo,
                    IsActive = true
                };

                await context.Students.AddAsync(student);
                await context.SaveChangesAsync();

                var createdStudent = await GetStudentWithDetails(student.Id);
                return ResponseDtoData<StudentDto>.SuccessResult(
                    mapper.Map<StudentDto>(createdStudent),
                    "Student created successfully",
                    (int)HttpStatusCode.Created
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating student: {Message}", ex.InnerException?.Message);
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"An error occurred while creating student: {ex.InnerException?.Message ?? ex.Message}"
                );
            }
        }

        public async Task<ResponseDtoData<StudentDto>> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await GetStudentWithDetails(id);
                if (student == null)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.NotFound,
                        "Student not found",
                        null);
                }

                return ResponseDtoData<StudentDto>.SuccessResult(
                    mapper.Map<StudentDto>(student),
                    "Student retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error retrieving student with ID {id}");
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred while retrieving student: {ex.Message}",
                    null);
            }
        }

        public async Task<ResponseDtoData<List<StudentDto>>> GetAllStudentsAsync()
        {
            try
            {
                var students = await context.Students
                    .AsNoTracking()
                    .Include(s => s.User)
                    .Include(s => s.Department)
                    .ToListAsync();

                var studentDtos = mapper.Map<List<StudentDto>>(students);

                return ResponseDtoData<List<StudentDto>>.SuccessResult(studentDtos, "Students retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all students");
                return ResponseDtoData<List<StudentDto>>.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred while retrieving students: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<List<StudentDto>>> GetActiveStudents()
        {
            try
            {
                var activeStudents = await context.Students
                    .AsNoTracking()
                    .Where(s => s.IsActive)
                    .Include(s => s.User)
                    .Include(s => s.Department)
                    .ToListAsync();

                var activeStudentDtos = mapper.Map<List<StudentDto>>(activeStudents);

                return ResponseDtoData<List<StudentDto>>.SuccessResult(activeStudentDtos, "Active students retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving active students");
                return ResponseDtoData<List<StudentDto>>.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred while retrieving active students: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<StudentDto>> UpdateStudentAsync(int id, StudentUpdateDto studentDto)
        {
            try
            {
                var student = await GetStudentWithDetails(id);
                if (student == null)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.NotFound,
                        "Student not found"
                    );
                }

                var user = await userManager.FindByIdAsync(student.UserId);
                if (user == null)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.NotFound,
                        "Associated user not found"
                    );
                }

                var updatedStudent = mapper.Map(studentDto, student);
                updatedStudent.UpdatedAt = DateTime.Now;
                context.Students.Update(updatedStudent);
                await context.SaveChangesAsync();

                var result = mapper.Map<StudentDto>(updatedStudent);
                return ResponseDtoData<StudentDto>.SuccessResult(result, "Student updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating student with ID {Id}: {Message}", id, ex.Message);
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"Error updating student: {ex.Message}"
                );
            }
        }

        public async Task<ResponseDto> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await context.Students.FindAsync(id);
                if (student == null)
                {
                    return ResponseDto.ErrorResult(
                        (int)HttpStatusCode.NotFound,
                        "Student not found");
                }

                student.IsActive = false;
                context.Students.Update(student);
                await context.SaveChangesAsync();

                var user = await userManager.FindByIdAsync(student.UserId);
                if (user != null)
                {
                    user.EmailConfirmed = false;
                    await userManager.UpdateAsync(user);
                }

                return ResponseDto.SuccessResult("Student deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting student with ID {id}");
                return ResponseDto.ErrorResult(
                    (int)HttpStatusCode.InternalServerError,
                    $"An unexpected error occurred while deleting student: {ex.Message}");
            }
        }

        private async Task<Student?> GetStudentWithDetails(int id)
        {
            if (id <= 0)
            {
                logger.LogWarning("Invalid student ID: {Id}", id);
                return null;
            }

            try
            {
                return await context.Students
                    .AsNoTracking()
                    .Include(s => s.User)
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching student with ID: {Id}", id);
                return null;
            }
        }
        #region Helper Methods
        private async Task<ResponseDtoData<StudentDto>?> ValidateStudentCreateDto(StudentCreateDto studentDto)
        {
            if (!await context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "Specified department does not exist",
                    null);
            }

            if (await context.Students.AnyAsync(s => s.AdmNo == studentDto.AdmNo))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "Student with this admission number already exists",
                    null);
            }

            if (await userManager.FindByEmailAsync(studentDto.Email) != null)
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "User with this email already exists",
                    null);
            }

            if (!int.TryParse(studentDto.NationalId, out _))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "National ID must be a valid number",
                    null);
            }

            return null;
        }
        #endregion
    }
}
