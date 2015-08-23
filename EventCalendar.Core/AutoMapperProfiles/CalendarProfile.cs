using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;

namespace EventCalendar.Core.AutoMapperProfiles
{
    public class CalendarProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ECalendar, CalendarDto>().ConvertUsing<CalendarDtoConverter>();
            Mapper.CreateMap<CalendarDto, ECalendar>().ConvertUsing<ECalendarConverter>();
        }

        private class CalendarDtoConverter : ITypeConverter<ECalendar, CalendarDto>
        {
            public CalendarDto Convert(ResolutionContext context)
            {
                var source = (ECalendar)context.SourceValue;
                var result = new CalendarDto() { 
                    Calendarname = source.Calendarname,
                    Color = source.Color,
                    DisplayOnSite = source.DisplayOnSite,
                    GCalFeedUrl = source.GCalFeedUrl,
                    GoogleAPIKey = source.GoogleAPIKey,
                    Id = source.Id,
                    IsGCal = source.IsGCal,
                    TextColor = source.TextColor,
                    ViewMode = source.ViewMode
                };
                return result;
            }
        }

        private class ECalendarConverter : ITypeConverter<CalendarDto, ECalendar>
        {
            public ECalendar Convert(ResolutionContext context)
            {
                var source = (CalendarDto)context.SourceValue;
                var result = new ECalendar()
                {
                    Calendarname = source.Calendarname,
                    Color = source.Color,
                    DisplayOnSite = source.DisplayOnSite,
                    GCalFeedUrl = source.GCalFeedUrl,
                    GoogleAPIKey = source.GoogleAPIKey,
                    Id = source.Id,
                    IsGCal = source.IsGCal,
                    TextColor = source.TextColor,
                    ViewMode = source.ViewMode
                };
                return result;
            }
        }
    }
}
