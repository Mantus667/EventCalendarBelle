using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
using System;
using System.Globalization;
using Umbraco.Core;

namespace EventCalendar.Core.AutoMapperProfiles
{
    public class EventProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Event, EventDto>().ConvertUsing<EventDtoConverter>();
            Mapper.CreateMap<EventDto, Event>().ConvertUsing<EventConverter>();
            Mapper.CreateMap<Event, EventDetailsModel>().ConvertUsing<EventDetailsConverter>();
        }

        private class EventDtoConverter : ITypeConverter<Event, EventDto>
        {
            public EventDto Convert(ResolutionContext context)
            {
                var source = (Event)context.SourceValue;
                var result = new EventDto()
                {
                    allDay = source.AllDay,
                    calendarId = source.calendarId,
                    categories = source.Categories,
                    end = source.End,
                    Id = source.Id,
                    locationId = source.locationId,
                    Organiser = source.Organiser,
                    start = source.Start,
                    title = source.Title,
                };
                return result;
            }
        }
        private class EventConverter : ITypeConverter<EventDto, Event>
        {
            public Event Convert(ResolutionContext context)
            {
                var source = (EventDto)context.SourceValue;
                var result = new Event()
                {
                    AllDay = source.allDay,
                    calendarId = source.calendarId,
                    Categories = source.categories,
                    End = source.end,
                    Id = source.Id,
                    locationId = source.locationId,
                    Organiser = source.Organiser,
                    Start = source.start,
                    Title = source.title
                };
                return result;
            }
        }
        private class EventDetailsConverter : ITypeConverter<Event, EventDetailsModel>
        {
            public EventDetailsModel Convert(ResolutionContext context)
            {
                var source = (Event)context.SourceValue;
                var calendar = CalendarService.GetCalendarById(source.calendarId);
                var ms = ApplicationContext.Current.Services.MemberService;

                var result = new EventDetailsModel() { 
                    CalendarId = calendar.Id,
                    CalendarName = calendar.Calendarname,
                    Title = source.Title,
                    Descriptions = source.Descriptions
                };
                if(source.locationId != 0){
                    result.LocationId = source.locationId;
                    result.Location = LocationService.GetLocation(source.locationId);
                }
                if (null != source.Start)
                {
                    result.StartDate = ((DateTime)source.Start).ToString("F", CultureInfo.CurrentCulture);
                }
                if (null != source.End)
                {
                    result.EndDate = ((DateTime)source.End).ToString("F", CultureInfo.CurrentCulture);
                }
                if (source.Organiser != null && source.Organiser != 0)
                {
                    var member = ms.GetById(source.Organiser);
                    result.Organiser = new Organiser() { Name = member.Name, Email = member.Email };
                }
                return result;
            }
        }
    }
}
