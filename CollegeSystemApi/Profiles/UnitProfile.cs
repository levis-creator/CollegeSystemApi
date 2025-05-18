using AutoMapper;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.Models.Entities;

namespace CollegeSystemApi.Profiles;

public class UnitProfile : Profile
{
    public UnitProfile()
    {
        CreateMap<CourseUnit, UnitDto>();
        CreateMap<CreateUnitDto, CourseUnit>();
        CreateMap<UpdateUnitDto, CourseUnit>();
        CreateMap<CourseUnit, UnitListDto>();
    }
}
