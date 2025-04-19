using System.Net;
using AutoMapper;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Programme;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Models.Enum;
using CollegeSystemApi.Services.Interfaces.IProgrammeServices;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services.ProgrammeServices;

public class ProgrammeService(
    ApplicationDbContext context,
    ILogger<ProgrammeService> logger,
    IMapper mapper)
    : IProgrammeService
{
    public async Task<ResponseDtoData<ProgrammeDto>> CreateProgrammeAsync(CreateProgrammeDto programmeDto)
    {
        try
        {
            bool exists = await context.Programmes.AnyAsync(p =>
                p.ProgrammeCode == programmeDto.ProgrammeCode || p.ProgrammeName == programmeDto.ProgrammeName);

            if (exists)
            {
                return ResponseDtoData<ProgrammeDto>.ErrorResult(
                    (int)HttpStatusCode.Conflict,
                    "Programme with this name or code already exists"
                );
            }

            // Parse level string to enum
            if (!Enum.TryParse<LevelType>(programmeDto.Level, true, out var levelEnum))
            {
                return ResponseDtoData<ProgrammeDto>.ErrorResult(
                    (int)HttpStatusCode.BadRequest,
                    "Invalid programme level"
                );
            }

            var programme = mapper.Map<Programme>(programmeDto);
            programme.Level = levelEnum;

            await context.Programmes.AddAsync(programme);
            await context.SaveChangesAsync();

            var createdDto = mapper.Map<ProgrammeDto>(programme);

            return ResponseDtoData<ProgrammeDto>.SuccessResult(createdDto, "Programme created successfully", (int)HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating programme");
            return ResponseDtoData<ProgrammeDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred while creating the programme: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDto> DeleteProgrammeAsync(int id)
    {
        try
        {
            var programme = await context.Programmes.FindAsync(id);
            if (programme == null)
            {
                return ResponseDto.ErrorResult((int)HttpStatusCode.NotFound, "Programme not found");
            }

            context.Programmes.Remove(programme);
            await context.SaveChangesAsync();

            return ResponseDto.SuccessResult("Programme deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting programme with ID {Id}", id);
            return ResponseDto.ErrorResult((int)HttpStatusCode.InternalServerError, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<ResponseDtoData<List<ProgrammeListDto>>> GetAllProgrammeListAsync()
    {
        try
        {
            var programmes = await context.Programmes
                .Include(p => p.Department)
                .AsNoTracking()
                .ToListAsync();

            var mapped = mapper.Map<List<ProgrammeListDto>>(programmes);

            return ResponseDtoData<List<ProgrammeListDto>>.SuccessResult(mapped, "Programme list retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving programme list");
            return ResponseDtoData<List<ProgrammeListDto>>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<List<ProgrammeDto>>> GetAllProgrammesAsync()
    {
        try
        {
            var programmes = await context.Programmes
                .Include(p => p.Department)
                .AsNoTracking()
                .ToListAsync();

            var result = mapper.Map<List<ProgrammeDto>>(programmes);

            return ResponseDtoData<List<ProgrammeDto>>.SuccessResult(result, "All programmes retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all programmes");
            return ResponseDtoData<List<ProgrammeDto>>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<ProgrammeDto>> GetProgrammeByIdAsync(int id)
    {
        try
        {
            var programme = await context.Programmes
                .Include(p => p.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (programme == null)
            {
                return ResponseDtoData<ProgrammeDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Programme not found"
                );
            }

            var dto = mapper.Map<ProgrammeDto>(programme);
            return ResponseDtoData<ProgrammeDto>.SuccessResult(dto, "Programme retrieved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving programme with ID {Id}", id);
            return ResponseDtoData<ProgrammeDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }

    public async Task<ResponseDtoData<ProgrammeDto>> UpdateProgrammeAsync(int id, UpdateProgrammeDto programmeDto)
    {
        try
        {
            var programme = await context.Programmes.FirstOrDefaultAsync(p => p.Id == id);

            if (programme == null)
            {
                return ResponseDtoData<ProgrammeDto>.ErrorResult(
                    (int)HttpStatusCode.NotFound,
                    "Programme not found"
                );
            }

            if (!string.IsNullOrWhiteSpace(programmeDto.ProgrammeName))
                programme.ProgrammeName = programmeDto.ProgrammeName;

            if (!string.IsNullOrWhiteSpace(programmeDto.ProgrammeCode))
                programme.ProgrammeCode = programmeDto.ProgrammeCode;

            if (!string.IsNullOrWhiteSpace(programmeDto.Level))
            {
                if (!Enum.TryParse<LevelType>(programmeDto.Level, true, out var levelEnum))
                {
                    return ResponseDtoData<ProgrammeDto>.ErrorResult(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid programme level"
                    );
                }
                programme.Level = levelEnum;
            }

            programme.ProgrammeDuration = programmeDto.ProgrammeDuration;
            programme.DepartmentId = programmeDto.DepartmentId;
            programme.Description = programmeDto.Description;
            programme.UpdatedAt = DateTime.Now;

            context.Programmes.Update(programme);
            await context.SaveChangesAsync();

            var updatedDto = mapper.Map<ProgrammeDto>(programme);

            return ResponseDtoData<ProgrammeDto>.SuccessResult(updatedDto, "Programme updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating programme with ID {Id}", id);
            return ResponseDtoData<ProgrammeDto>.ErrorResult(
                (int)HttpStatusCode.InternalServerError,
                $"An error occurred: {ex.Message}"
            );
        }
    }
}
