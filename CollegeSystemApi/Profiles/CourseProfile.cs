using AutoMapper;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.DTOs.Courses;
using CollegeSystemApi.Models;

namespace CollegeSystemApi.Profiles;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.DepartmentName));

        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>();
        CreateMap<Course, CourseListDto>();
    }
}
