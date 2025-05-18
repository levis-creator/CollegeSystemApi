using System.Net;
using AutoMapper;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Timetable;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services
{
    public class TimetableService(
        ApplicationDbContext context,
        ILogger<TimetableService> logger,
        IMapper mapper)
        : ITimetableService
    {
        public async Task<ResponseDtoData<TimetableDto>> CreateTimetableAsync(CreateTimetableDto dto)
        {
            try
            {
                var academicYear = await context.AcademicYears.FindAsync(dto.AcademicYearId);
                if (academicYear == null)
                {
                    return ResponseDtoData<TimetableDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Academic Year ID");
                }

                var timetable = new TimeTable
                {
                    AcademicYear = academicYear
                };

                await context.TimeTables.AddAsync(timetable);
                await context.SaveChangesAsync();

                var result = mapper.Map<TimetableDto>(timetable);

                return ResponseDtoData<TimetableDto>.SuccessResult(result, "Timetable created successfully", (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating timetable");
                return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<TimetableDto>> GetTimetableByIdAsync(int id)
        {
            try
            {
                var timetable = await context.TimeTables
                    .Include(t => t.AcademicYear)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (timetable == null)
                {
                    return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.NotFound, "Timetable not found");
                }

                var dto = mapper.Map<TimetableDto>(timetable);
                return ResponseDtoData<TimetableDto>.SuccessResult(dto, "Timetable retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving timetable with ID {Id}", id);
                return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<List<TimetableDto>>> GetAllTimetablesAsync()
        {
            try
            {
                var timetables = await context.TimeTables
                    .Include(t => t.AcademicYear)
                    .AsNoTracking()
                    .ToListAsync();

                var dtos = mapper.Map<List<TimetableDto>>(timetables);
                return ResponseDtoData<List<TimetableDto>>.SuccessResult(dtos, "Timetables retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving timetables");
                return ResponseDtoData<List<TimetableDto>>.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<TimetableDto>> UpdateTimetableAsync(int id, UpdateTimetableDto dto)
        {
            try
            {
                var timetable = await context.TimeTables.Include(t => t.AcademicYear).FirstOrDefaultAsync(t => t.Id == id);
                if (timetable == null)
                {
                    return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.NotFound, "Timetable not found");
                }

                var academicYear = await context.AcademicYears.FindAsync(dto.AcademicYearId);
                if (academicYear == null)
                {
                    return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.BadRequest, "Invalid Academic Year ID");
                }

                timetable.AcademicYear = academicYear;
                timetable.UpdatedAt = DateTime.UtcNow;

                context.TimeTables.Update(timetable);
                await context.SaveChangesAsync();

                var result = mapper.Map<TimetableDto>(timetable);
                return ResponseDtoData<TimetableDto>.SuccessResult(result, "Timetable updated successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating timetable with ID {Id}", id);
                return ResponseDtoData<TimetableDto>.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDto> DeleteTimetableAsync(int id)
        {
            try
            {
                var timetable = await context.TimeTables.FindAsync(id);
                if (timetable == null)
                {
                    return ResponseDto.ErrorResult((int)HttpStatusCode.NotFound, "Timetable not found");
                }

                context.TimeTables.Remove(timetable);
                await context.SaveChangesAsync();

                return ResponseDto.SuccessResult("Timetable deleted successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting timetable with ID {Id}", id);
                return ResponseDto.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        public async Task<ResponseDtoData<TimetableWithSchedulesDto>> GetTimetableWithSchedulesAsync(int id)
        {
            try
            {
                var timetable = await context.TimeTables
                    .Include(t => t.AcademicYear)
                    .Include(t => t.Schedules)
                        .ThenInclude(s => s.Classroom)
                    .Include(t => t.Schedules)
                        .ThenInclude(s => s.CourseUnit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (timetable == null)
                {
                    return ResponseDtoData<TimetableWithSchedulesDto>.ErrorResult((int)HttpStatusCode.NotFound, "Timetable not found");
                }

                var dto = mapper.Map<TimetableWithSchedulesDto>(timetable);
                return ResponseDtoData<TimetableWithSchedulesDto>.SuccessResult(dto, "Timetable with schedules retrieved successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving timetable with schedules for ID {Id}", id);
                return ResponseDtoData<TimetableWithSchedulesDto>.ErrorResult((int)HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
