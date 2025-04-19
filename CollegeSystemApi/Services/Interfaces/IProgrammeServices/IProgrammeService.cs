using CollegeSystemApi.DTOs.Programme;
using CollegeSystemApi.DTOs.Response;

namespace CollegeSystemApi.Services.Interfaces.IProgrammeServices;

public interface IProgrammeService
{
    Task<ResponseDtoData<ProgrammeDto>> CreateProgrammeAsync(CreateProgrammeDto programmeDto);
    Task<ResponseDtoData<ProgrammeDto>> GetProgrammeByIdAsync(int id);
    Task<ResponseDtoData<List<ProgrammeDto>>> GetAllProgrammesAsync();
    Task<ResponseDtoData<List<ProgrammeListDto>>> GetAllProgrammeListAsync();
    Task<ResponseDtoData<ProgrammeDto>> UpdateProgrammeAsync(int id, UpdateProgrammeDto programmeDto);
    Task<ResponseDto> DeleteProgrammeAsync(int id);
}