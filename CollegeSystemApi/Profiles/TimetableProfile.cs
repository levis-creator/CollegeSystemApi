using AutoMapper;
using CollegeSystemApi.DTOs.Timetable;
using CollegeSystemApi.Models.Entities;

namespace CollegeSystemApi.Profiles;

public class TimetableProfile:Profile
{
    public TimetableProfile()
    {
        CreateMap<TimeTable, TimetableDto>()
               .ForMember(dest => dest.AcademicPeriod, opt =>
                   opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.AcademicPeriod : string.Empty));
        // Map CreateTimetableDto to TimeTable
        CreateMap<CreateTimetableDto, TimeTable>()
            .ForMember(dest => dest.AcademicYear,
                       opt => opt.Ignore()); // You will assign this manually from DB context

        // Map UpdateTimetableDto to TimeTable
        CreateMap<UpdateTimetableDto, TimeTable>()
            .ForMember(dest => dest.AcademicYear,
                       opt => opt.Ignore()); // Also set manually

        // Map TimeTable to TimetableWithSchedulesDto
        CreateMap<TimeTable, TimetableWithSchedulesDto>()
            .ForMember(dest => dest.AcademicPeriod,
                       opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.AcademicPeriod : string.Empty))
            .ForMember(dest => dest.Schedules,
                       opt => opt.MapFrom(src => src.Schedules));
    }
}
