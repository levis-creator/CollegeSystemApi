using AutoMapper;
using CollegeSystemApi.DTOs.Programme;
using CollegeSystemApi.Models.Entities;
using CollegeSystemApi.Models.Enum;

namespace CollegeSystemApi.Profiles;

public class ProgrammeProfile : Profile
{
    public ProgrammeProfile()
    {
        // Create and Update DTOs → Entity
        CreateMap<CreateProgrammeDto, Programme>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<LevelType>(src.Level, true)));

        CreateMap<UpdateProgrammeDto, Programme>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => Enum.Parse<LevelType>(src.Level, true)));

        // Entity → Read DTOs
        CreateMap<Programme, ProgrammeDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : string.Empty));

        CreateMap<Programme, ProgrammeListDto>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DepartmentName : string.Empty));
    }
}
