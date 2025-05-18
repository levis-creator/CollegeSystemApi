using AutoMapper;
using CollegeSystemApi.DTOs.Schedule;
using CollegeSystemApi.Models.Entities;

namespace CollegeSystemApi.Profiles;

public class ScheduleProfile:Profile
{
    public ScheduleProfile()
    {
        // Map Schedule to ScheduleDto
        CreateMap<Schedule, ScheduleDto>()
                  .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.Day.ToString()))
                  .ForMember(dest => dest.CourseUnitName, opt => opt.MapFrom(src => src.CourseUnit != null ? src.CourseUnit.UnitName : string.Empty))
                  .ForMember(dest => dest.CourseUnitCode, opt => opt.MapFrom(src => src.CourseUnit != null ? src.CourseUnit.UnitCode : string.Empty))
                  .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.ClassName : string.Empty))
                  .ForMember(dest => dest.ClassroomShortName, opt => opt.MapFrom(src => src.Classroom != null ? src.Classroom.ShortName : string.Empty));

        CreateMap<CreateScheduleDto, Schedule>()
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => Enum.Parse<DayOfWeek>(src.Day, true)));

        CreateMap<UpdateScheduleDto, Schedule>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
