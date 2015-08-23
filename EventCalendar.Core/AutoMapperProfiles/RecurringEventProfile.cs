using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;

namespace EventCalendar.Core.AutoMapperProfiles
{
    public class RecurringEventProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<RecurringEventDto, RecurringEvent>().ConvertUsing<RecurringEventConverter>();
            Mapper.CreateMap<RecurringEvent, RecurringEventDto>().ConvertUsing<RecurringEventDtoConverter>();
            Mapper.CreateMap<RecurringEvent, EventDetailsModel>().ConvertUsing<RecurringEventDetailsConverter>();
        }

        private class RecurringEventConverter : ITypeConverter<RecurringEventDto, RecurringEvent> {

            public RecurringEvent Convert(ResolutionContext context)
            {
                var source = (RecurringEventDto)context.SourceValue;
                var result = new RecurringEvent()
                {
                    Id = source.Id,
                    Organiser = source.Organiser,
                    AllDay = source.allDay,
                    calendarId = source.calendarId,
                    Categories = source.categories,
                    Days = source.day.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList(),
                    End = source.end,
                    Frequency = source.frequency,
                    locationId = source.locationId,
                    MonthlyIntervals = source.monthly_interval.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList(),
                    range_end = source.range_end,
                    range_start = source.range_start,
                    Start = source.start,
                    Title = source.title,
                    Exceptions = DateExceptionService.GetDateExceptionsForRecurringEvent(source.Id).ToList()
                };

                return result;
            }
        }
        private class RecurringEventDtoConverter : ITypeConverter<RecurringEvent, RecurringEventDto>
        {

            RecurringEventDto ITypeConverter<RecurringEvent, RecurringEventDto>.Convert(ResolutionContext context)
            {
                var source = (RecurringEvent)context.SourceValue;
                var result = new RecurringEventDto()
                {
                    Id = source.Id,
                    locationId = source.locationId,
                    allDay = source.AllDay,
                    calendarId = source.calendarId,
                    categories = source.Categories,
                    day = string.Join(",", source.Days.ToArray()),
                    end = source.End,
                    frequency = source.Frequency,
                    monthly_interval = string.Join(",", source.MonthlyIntervals.ToArray()),
                    Organiser = source.Organiser,
                    range_end = source.range_end,
                    range_start = source.range_start,
                    start = source.Start,
                    title = source.Title,
                };
                return result;
            }
        }
        private class RecurringEventDetailsConverter : ITypeConverter<RecurringEvent, EventDetailsModel>
        {
            public EventDetailsModel Convert(ResolutionContext context)
            {
                var ms = ApplicationContext.Current.Services.MemberService;

                var source = (RecurringEvent)context.SourceValue;

                var result = new EventDetailsModel()
                {
                    Title = source.Title,
                    Descriptions = source.Descriptions
                };

                if (source.locationId != 0)
                {
                    result.Location = LocationService.GetLocation(source.locationId);
                }

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = source.Title,
                    FrequencyTypeOptions = (FrequencyTypeEnum)source.Frequency
                };

                foreach (var day in source.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in source.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                Schedule schedule = new Schedule(tmp_event);

                if (null != source.Start)
                {
                    var start = ((DateTime)schedule.NextOccurrence(DateTime.Now));
                    result.StartDate = new DateTime(start.Year, start.Month, start.Day, source.Start.Hour, source.Start.Minute, 0).ToString("F", CultureInfo.CurrentCulture);

                }
                if (null != source.End)
                {
                    var end = ((DateTime)schedule.NextOccurrence(DateTime.Now));
                    result.EndDate = new DateTime(end.Year, end.Month, end.Day, source.End.Hour, source.End.Minute, 0).ToString("F", CultureInfo.CurrentCulture);
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
