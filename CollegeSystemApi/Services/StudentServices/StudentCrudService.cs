using System.Net;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.Models;
using CollegeSystemApi.Services.Interfaces.IStudentServices;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services.StudentServices
{
    public class StudentCrudService(
        UserManager<AppUser> userManager,
        ApplicationDbContext context,
        ILogger<StudentCrudService> logger)
        : IStudentCrudService
    {
        public async Task<ResponseDtoData<StudentDto>> CreateStudentAsync(StudentCreateDto studentDto)
        {
            try
            {
                // Validate input
                var validationResult = await ValidateStudentCreateDto(studentDto);
                if (validationResult != null)
                    return validationResult;

                // Ensure National ID is a valid integer
                if (!int.TryParse(studentDto.NationalId, out int nationalId))
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid National ID format."
                    );
                }

                // Check if student already exists
                bool studentExists = await context.Students.AnyAsync(s => s.NationalId == nationalId);
                if (studentExists)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.Conflict,
                        "A student with this National ID already exists."
                    );
                }

                // Validate Department ID
                bool departmentExists = await context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId);
                if (!departmentExists)
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Department ID."
                    );
                }

                // Create AppUser first
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

                // Create user with National ID as initial password
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

                // Add student role
                var roleResult = await userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Role assignment failed: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    await userManager.DeleteAsync(user); // Rollback user creation if role assignment fails
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.InternalServerError,
                        "Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description))
                    );
                }

                // Create Student record
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

                // Return created student
                var createdStudent = await GetStudentWithDetails(student.Id);
                return ResponseDtoData<StudentDto>.SuccessResult(
                    MapToStudentDto(createdStudent!),
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
                    MapToStudentDto(student),
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

                var studentDtos = students.Select(MapToStudentDto).ToList();

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

                var activeStudentDtos = activeStudents.Select(MapToStudentDto).ToList();

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
        
                bool isStudentUpdated = false;
                bool isUserUpdated = false;

                // Department update
                if (studentDto.DepartmentId != 0 && studentDto.DepartmentId != student.DepartmentId)
                {
                    var departmentExists = await context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId);
                    if (!departmentExists)
                    {
                        return ResponseDtoData<StudentDto>.ErrorResult(
                            (int)HttpStatusCode.BadRequest,
                            "Specified department does not exist"
                        );
                    }
                    student.Department = null;
                    student.DepartmentId = studentDto.DepartmentId;
                    isStudentUpdated = true;
                }

                // National ID update
                if (!string.IsNullOrWhiteSpace(studentDto.NationalId) &&
                    int.TryParse(studentDto.NationalId, out int nationalId) &&
                    student.NationalId != nationalId)
                {
                    student.NationalId = nationalId;
                    isStudentUpdated = true;
                }
                else if (!string.IsNullOrWhiteSpace(studentDto.NationalId) && !int.TryParse(studentDto.NationalId, out _))
                {
                    return ResponseDtoData<StudentDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "National ID must be a valid number"
                    );
                }

                // AdmNo update
                if (!string.IsNullOrWhiteSpace(studentDto.AdmNo) && studentDto.AdmNo != student.AdmNo)
                {
                    student.AdmNo = studentDto.AdmNo;
                    isStudentUpdated = true;
                }

                // First name
                if (!string.IsNullOrWhiteSpace(studentDto.FirstName) && studentDto.FirstName != user.FirstName)
                {
                    user.FirstName = studentDto.FirstName;
                    isUserUpdated = true;
                }

                // Last name
                if (!string.IsNullOrWhiteSpace(studentDto.LastName) && studentDto.LastName != user.LastName)
                {
                    user.LastName = studentDto.LastName;
                    isUserUpdated = true;
                }

                // Email
                if (!string.IsNullOrWhiteSpace(studentDto.Email) && studentDto.Email != user.Email)
                {
                    var emailExists = await userManager.FindByEmailAsync(studentDto.Email);
                    if (emailExists != null && emailExists.Id != user.Id)
                    {
                        return ResponseDtoData<StudentDto>.ErrorResult(
                            (int)HttpStatusCode.Conflict,
                            "Email already in use by another user"
                        );
                    }

                    user.Email = studentDto.Email;
                    user.UserName = studentDto.Email;
                    isUserUpdated = true;
                }

                // Update only if changes were made
                if (isUserUpdated)
                {
                    var userResult = await userManager.UpdateAsync(user);
                    if (!userResult.Succeeded)
                    {
                        return ResponseDtoData<StudentDto>.ErrorResult(
                            (int)HttpStatusCode.BadRequest,
                            "User update failed: " + string.Join(", ", userResult.Errors.Select(e => e.Description))
                        );
                    }
                }

                    context.Entry(user).State = EntityState.Detached;
                if (isStudentUpdated)
                {
                    student.UpdatedAt = DateTime.Now;
                    context.Students.Update(student);
                    await context.SaveChangesAsync();
                }

                var updatedStudent = await GetStudentWithDetails(id);
                return ResponseDtoData<StudentDto>.SuccessResult(
                    MapToStudentDto(updatedStudent!),
                    "Student updated successfully"
                );
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

                // Soft delete
                student.IsActive = false;
                context.Students.Update(student);
                await context.SaveChangesAsync();

                // Optionally deactivate the user as well
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


        #region Helper Methods

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
                    .AsNoTracking() // Improves performance if no updates are needed
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

        private async Task<ResponseDtoData<StudentDto>?> ValidateStudentCreateDto(StudentCreateDto studentDto)
        {
            // Validate department exists
            if (!await context.Departments.AnyAsync(d => d.Id == studentDto.DepartmentId))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "Specified department does not exist",
                    null);
            }

            // Check for duplicate admission number
            if (await context.Students.AnyAsync(s => s.AdmNo == studentDto.AdmNo))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "Student with this admission number already exists",
                    null);
            }

            // Check for duplicate email
            if (await userManager.FindByEmailAsync(studentDto.Email) != null)
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "User with this email already exists",
                    null);
            }

            // Validate National ID format
            if (!int.TryParse(studentDto.NationalId, out _))
            {
                return ResponseDtoData<StudentDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "National ID must be a valid number",
                    null);
            }

            return null;
        }

        private static StudentDto MapToStudentDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id.ToString(),
                FirstName = student.User?.FirstName ?? string.Empty,
                LastName = student.User?.LastName ?? string.Empty,
                Email = student.User?.Email ?? string.Empty,
                NationalId = student.NationalId.ToString(),
                AdmNo = student.AdmNo,
                DepartmentId = student.DepartmentId.ToString(),
                DepartmentName = student.Department?.DepartmentName,
                DepartmentCode = student.Department?.DepartmentCode,
                IsActive = student.IsActive
            };
        }

        #endregion
    }
}