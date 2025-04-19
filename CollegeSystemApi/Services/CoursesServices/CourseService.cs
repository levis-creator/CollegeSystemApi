using System.Net;
using AutoMapper;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces.ICoursesServices;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services.CoursesServices;

public class CourseService(
    ApplicationDbContext context,
    ILogger<CourseService> logger,
    IMapper mapper)
    : ICourseService
{
    public async Task<ResponseDtoData<CourseDto>> CreateCourseAsync(CreateCourseDto courseDto)
    {
        try
        {
            // Check if course code or name already exists
            bool exists = await context.Courses.AnyAsync(c =>
                c.CourseCode == courseDto.CourseCode || c.CourseName == courseDto.CourseName);

            if (exists)
            {
                return ResponseDtoData<CourseDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "Course with this name or code already exists"
                );
            }

            var course = mapper.Map<Course>(courseDto);

            await context.Courses.AddAsync(course);
            await context.SaveChangesAsync();

            var createdDto = mapper.Map<CourseDto>(course);

            return ResponseDtoData<CourseDto>.SuccessResult(createdDto, "Course created successfully", (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating course");
            return ResponseDtoData<CourseDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred while creating the course: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDto> DeleteCourseAsync(int id)
    {
        try
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null)
            {
                return ResponseDto.ErrorResult((int)HttpStatusCode.NotFound, "Course not found");
            }

            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            return ResponseDto.SuccessResult("Course deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting course with ID {Id}", id);
            return ResponseDto.ErrorResult((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<ResponseDtoData<List<CourseDto>>> GetAllCoursesAsync()
    {
        try
        {
            var allCourses = await context.Courses.Include(x=>x.Department).AsNoTracking().ToListAsync();
            var result = mapper.Map<List<CourseDto>>(allCourses);

            return ResponseDtoData<List<CourseDto>>.SuccessResult(result, "All courses retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all courses");
            return ResponseDtoData<List<CourseDto>>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<CourseDto>> GetCourseByIdAsync(int id)
    {
        try
        {
            var course = await context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ResponseDtoData<CourseDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Course not found"
                );
            }

            var result = mapper.Map<CourseDto>(course);
            return ResponseDtoData<CourseDto>.SuccessResult(result, "Course retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving course with ID {Id}", id);
            return ResponseDtoData<CourseDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<List<CourseListDto>>> GetAllCourseListAsync()
    {
        var courses = await context.Courses
            .Include(c => c.Department)
            .ToListAsync();

        var mappedCourses = mapper.Map<List<CourseListDto>>(courses);

        return ResponseDtoData<List<CourseListDto>>.SuccessResult(message: "Courses retrieved successfully",
            data: mappedCourses);
    }

    public async Task<ResponseDtoData<CourseDto>> UpdateCourseAsync(int id, UpdateCourseDto courseDto)
    {
        try
        {
            var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ResponseDtoData<CourseDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Course not found"
                );
            }

            if (!string.IsNullOrWhiteSpace(courseDto.CourseCode))
                course.CourseCode = courseDto.CourseCode;

            if (!string.IsNullOrWhiteSpace(courseDto.CourseName))
                course.CourseName = courseDto.CourseName;

            course.UpdatedAt = DateTime.Now;

            context.Courses.Update(course);
            await context.SaveChangesAsync();

            var updatedDto = mapper.Map<CourseDto>(course);

            return ResponseDtoData<CourseDto>.SuccessResult(updatedDto, "Course updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating course with ID {Id}", id);
            return ResponseDtoData<CourseDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }
}
