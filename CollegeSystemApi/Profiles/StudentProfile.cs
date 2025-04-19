using AutoMapper;
using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.Models.Entities;

namespace CollegeSystemApi.Profiles;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        // Mapping from StudentCreateDto to Student
        CreateMap<StudentCreateDto, Student>()
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => int.Parse(src.NationalId)));

        // Mapping from StudentUpdateDto to Student
        CreateMap<StudentUpdateDto, Student>()
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => int.Parse(src.NationalId)));

        // Mapping from Student to StudentDto
        CreateMap<Student, StudentDto>()
              .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
              .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
              .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId.ToString()))
              .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId.ToString()))
              .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.DepartmentName))
              .ForMember(dest => dest.DepartmentCode, opt => opt.MapFrom(src => src.Department!.DepartmentCode))
              .ForMember(dest => dest.ProgrammeId, opt => opt.MapFrom(src => src.ProgrammeId.ToString()))
              .ForMember(dest => dest.ProgrammeCode, opt => opt.MapFrom(src => src.Programme!.ProgrammeCode));
        // Optional: reverse mappings
        CreateMap<StudentDto, Student>();
    }
}
