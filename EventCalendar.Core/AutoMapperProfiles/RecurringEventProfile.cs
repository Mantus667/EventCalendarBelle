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
using Umbraco.Web;

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
                    Icon = String.IsNullOrEmpty(source.icon) ? String.Empty : source.icon,
                    Exceptions = DateExceptionService.GetDateExceptionsForRecurringEvent(source.Id).ToList()
                };

                result.Calendar = CalendarService.GetCalendarById(result.calendarId);

                if (result.locationId != 0)
                {
                    result.Location = LocationService.GetLocation(result.locationId);
                }
                result.MediaItems = new List<int>();
                if (!String.IsNullOrEmpty(source.media))
                {
                    result.MediaItems = source.media.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
                }

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
                    day = String.Join(",", source.Days.ToArray()),
                    end = source.End,
                    frequency = source.Frequency,
                    monthly_interval = String.Join(",", source.MonthlyIntervals.ToArray()),
                    Organiser = source.Organiser,
                    range_end = source.range_end,
                    range_start = source.range_start,
                    start = source.Start,
                    title = source.Title,
                    icon = source.Icon,
                };

                if (source.MediaItems != null)
                {
                    result.media = String.Join(",", source.MediaItems.ToArray());
                }
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

                RangeInYear rangeInYear = null;

                if (source.range_start != 0 && source.range_end != 0)
                {
                    rangeInYear = new RangeInYear()
                    {
                        StartMonth = source.range_start,
                        EndMonth = source.range_end
                    };

                    tmp_event.RangeInYear = rangeInYear;
                }

                Schedule schedule = new Schedule(tmp_event,source.Exceptions.Select(x => (DateTime)x.Date));
                var today = DateTime.Now;

                if (schedule.IsOccurring(today))
                {
                    result.StartDate = new DateTime(today.Year, today.Month, today.Day, source.Start.Hour, source.Start.Minute, 0).ToString("F", CultureInfo.CurrentCulture);
                    result.EndDate = new DateTime(today.Year, today.Month, today.Day, source.End.Hour, source.End.Minute, 0).ToString("F", CultureInfo.CurrentCulture);
                    if (today.Hour > source.End.Hour && today.Minute > source.End.Minute)
                    {
                        result.isOver = true;
                    }
                }
                else
                {
                    if (null != source.Start)
                    {
                        var start = ((DateTime)schedule.NextOccurrence(today));
                        result.StartDate = new DateTime(start.Year, start.Month, start.Day, source.Start.Hour, source.Start.Minute, 0).ToString("F", CultureInfo.CurrentCulture);

                    }
                    if (null != source.End)
                    {
                        var end = ((DateTime)schedule.NextOccurrence(today));
                        result.EndDate = new DateTime(end.Year, end.Month, end.Day, source.End.Hour, source.End.Minute, 0).ToString("F", CultureInfo.CurrentCulture);
                    }
                }
                if (source.Organiser != 0)
                {
                    var member = ms.GetById(source.Organiser);
                    result.Organiser = new Organiser() { Name = member.Name, Email = member.Email };
                }

                result.MediaItems = new List<Umbraco.Core.Models.IPublishedContent>();
                if (source.MediaItems.Any())
                {
                    var helper = new UmbracoHelper(UmbracoContext.Current);
                    result.MediaItems = helper.TypedMedia(source.MediaItems).ToList();
                }

                return result;
            }
        }
    }
}
