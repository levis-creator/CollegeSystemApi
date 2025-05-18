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
    public async Task<ResponseDtoData<UnitDto>> CreateCourseAsync(CreateUnitDto courseDto)
    {
        try
        {
            // Check if course code or name already exists
            bool exists = await context.CourseUnits.AnyAsync(c =>
                c.UnitCode == courseDto.UnitCode || c.UnitName == courseDto.UnitName);

            if (exists)
            {
                return ResponseDtoData<UnitDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "Course with this name or code already exists"
                );
            }

            var course = mapper.Map<CourseUnit>(courseDto);

            await context.CourseUnits.AddAsync(course);
            await context.SaveChangesAsync();

            var createdDto = mapper.Map<UnitDto>(course);

            return ResponseDtoData<UnitDto>.SuccessResult(createdDto, "Course created successfully", (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating course");
            return ResponseDtoData<UnitDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred while creating the course: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDto> DeleteCourseAsync(int id)
    {
        try
        {
            var course = await context.CourseUnits.FindAsync(id);
            if (course == null)
            {
                return ResponseDto.ErrorResult((int)HttpStatusCode.NotFound, "Course not found");
            }

            context.CourseUnits.Remove(course);
            await context.SaveChangesAsync();

            return ResponseDto.SuccessResult("Course deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting course with ID {Id}", id);
            return ResponseDto.ErrorResult((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<ResponseDtoData<List<UnitDto>>> GetAllCoursesAsync()
    {
        try
        {
            var allCourses = await context.CourseUnits.AsNoTracking().ToListAsync();
            var result = mapper.Map<List<UnitDto>>(allCourses);

            return ResponseDtoData<List<UnitDto>>.SuccessResult(result, "All courses retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all courses");
            return ResponseDtoData<List<UnitDto>>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<UnitDto>> GetCourseByIdAsync(int id)
    {
        try
        {
            var course = await context.CourseUnits.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ResponseDtoData<UnitDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Course not found"
                );
            }

            var result = mapper.Map<UnitDto>(course);
            return ResponseDtoData<UnitDto>.SuccessResult(result, "Course retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving course with ID {Id}", id);
            return ResponseDtoData<UnitDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<List<UnitListDto>>> GetAllCourseListAsync()
    {
        var courses = await context.CourseUnits
            .ToListAsync();

        var mappedCourses = mapper.Map<List<UnitListDto>>(courses);

        return ResponseDtoData<List<UnitListDto>>.SuccessResult(message: "CourseUnits retrieved successfully",
            data: mappedCourses);
    }

    public async Task<ResponseDtoData<UnitDto>> UpdateCourseAsync(int id, UpdateUnitDto courseDto)
    {
        try
        {
            var course = await context.CourseUnits.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return ResponseDtoData<UnitDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Course not found"
                );
            }

            if (!string.IsNullOrWhiteSpace(courseDto.UnitCode))
                course.UnitCode = courseDto.UnitCode;

            if (!string.IsNullOrWhiteSpace(courseDto.UnitName))
                course.UnitName = courseDto.UnitName;

            course.UpdatedAt = DateTime.Now;

            context.CourseUnits.Update(course);
            await context.SaveChangesAsync();

            var updatedDto = mapper.Map<UnitDto>(course);

            return ResponseDtoData<UnitDto>.SuccessResult(updatedDto, "Course updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating course with ID {Id}", id);
            return ResponseDtoData<UnitDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }
}
