using System.Net;
using AutoMapper;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.DTOs.Schedule;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services;

public class ScheduleService(
    ApplicationDbContext context,
    ILogger<ScheduleService> logger,
    IMapper mapper) : IScheduleService
{
    public async Task<ResponseDtoData<ScheduleDto>> CreateScheduleAsync(CreateScheduleDto dto)
    {
        try
        {
            var schedule = mapper.Map<Schedule>(dto);

            // Ensure related entities exist
            if (!await context.CourseUnits.AnyAsync(c => c.Id == dto.CourseUnitId) ||
                !await context.Classrooms.AnyAsync(c => c.Id == dto.ClassroomId))
            {
                return ResponseDtoData<ScheduleDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "Invalid CourseUnitId or ClassroomId."
                );
            }

            await context.Schedules.AddAsync(schedule);
            await context.SaveChangesAsync();

            var result = await context.Schedules
                .Include(s => s.CourseUnit)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(s => s.Id == schedule.Id);

            return ResponseDtoData<ScheduleDto>.SuccessResult(
                mapper.Map<ScheduleDto>(result!),
                "Schedule created successfully",
                (int)HttpStatusCode.Created
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating schedule");
            return ResponseDtoData<ScheduleDto>.ErrorResult(500, ex.Message);
        }
    }

    public async Task<ResponseDtoData<ScheduleDto>> GetScheduleByIdAsync(int id)
    {
        var schedule = await context.Schedules
            .Include(s => s.CourseUnit)
            .Include(s => s.Classroom)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
            return ResponseDtoData<ScheduleDto>.ErrorResult(404, "Schedule not found");

        return ResponseDtoData<ScheduleDto>.SuccessResult(
            mapper.Map<ScheduleDto>(schedule),
            "Schedule retrieved successfully"
        );
    }

    public async Task<ResponseDtoData<List<ScheduleDto>>> GetAllSchedulesAsync()
    {
        var schedules = await context.Schedules
            .Include(s => s.CourseUnit)
            .Include(s => s.Classroom)
            .AsNoTracking()
            .ToListAsync();

        var result = mapper.Map<List<ScheduleDto>>(schedules);
        return ResponseDtoData<List<ScheduleDto>>.SuccessResult(result, "Schedules retrieved successfully");
    }

    public async Task<ResponseDtoData<ScheduleDto>> UpdateScheduleAsync(int id, UpdateScheduleDto dto)
    {
        var schedule = await context.Schedules.FindAsync(id);
        if (schedule == null)
            return ResponseDtoData<ScheduleDto>.ErrorResult(404, "Schedule not found");

        if (dto.CourseUnitId.HasValue && !await context.CourseUnits.AnyAsync(c => c.Id == dto.CourseUnitId))
            return ResponseDtoData<ScheduleDto>.ErrorResult(400, "Invalid CourseUnitId");

        if (dto.ClassroomId.HasValue && !await context.Classrooms.AnyAsync(c => c.Id == dto.ClassroomId))
            return ResponseDtoData<ScheduleDto>.ErrorResult(400, "Invalid ClassroomId");

        // Map updated values
        mapper.Map(dto, schedule);
        schedule.UpdatedAt = DateTime.Now;

        context.Schedules.Update(schedule);
        await context.SaveChangesAsync();

        var updated = await context.Schedules
            .Include(s => s.CourseUnit)
            .Include(s => s.Classroom)
            .FirstOrDefaultAsync(s => s.Id == id);

        return ResponseDtoData<ScheduleDto>.SuccessResult(mapper.Map<ScheduleDto>(updated!), "Schedule updated successfully");
    }

    public async Task<ResponseDto> DeleteScheduleAsync(int id)
    {
        var schedule = await context.Schedules.FindAsync(id);
        if (schedule == null)
            return ResponseDto.ErrorResult(404, "Schedule not found");

        context.Schedules.Remove(schedule);
        await context.SaveChangesAsync();

        return ResponseDto.SuccessResult("Schedule deleted successfully");
    }
}
