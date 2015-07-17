using EventCalendar.Core.ActionFilter;
using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
using Newtonsoft.Json;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Umbraco.Core.Persistence;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class ECApiController : UmbracoApiController
    {
        public string sourcePrefix { get; private set; }

        public ECApiController()
        {
            if(WebConfigurationManager.AppSettings.AllKeys.Contains("EventCalendar:EventSourcePrefix")){
                sourcePrefix = WebConfigurationManager.AppSettings["EventCalendar:EventSourcePrefix"];
            }
        }

        [HttpPost]
        [ValidateHttpAntiForgeryToken]
        public IEnumerable<EventsOverviewModel> CalendarEvents(EventRange range)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            List<EventsOverviewModel> events = new List<EventsOverviewModel>();

            if (range.id != 0)
            {
                events.AddRange(this.GetNormalEvents(range.id, range.culture ?? "en-us", range.start, range.end));
                events.AddRange(this.GetRecurringEvents(range.id, range.culture ?? "en-us", range.start, range.end));
            }
            else
            {
                var calendar = db.Query<ECalendar>("SELECT * FROM ec_calendars").ToList();
                foreach (var cal in calendar)
                {
                    events.AddRange(this.GetNormalEvents(cal.Id, range.culture ?? "en-us", range.start, range.end));
                    events.AddRange(this.GetRecurringEvents(cal.Id, range.culture ?? "en-us", range.start, range.end));
                }
            }

            return events;
        }

        [HttpGet]
        public List<object> GetCalendarSources(int id = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            List<object> sources = new List<object>();

            //Check if we got an id, otherwise get all calendar
            if (id != 0)
            {
                var calendar = CalendarService.GetCalendarById(id);
                if (calendar.IsGCal)
                {
                    sources.Add(calendar.GCalFeedUrl);
                }
                else
                {
                    sources.Add(new EventSource { 
                        url = (sourcePrefix ?? String.Empty) + "/umbraco/EventCalendar/ECApi/CalendarEvents/",
                        data = new { id = calendar.Id }
                    });
                }
                
            }
            else
            {
                //Get all calendar
                var calendar = CalendarService.GetAllCalendar();

                foreach (var cal in calendar)
                {
                    if (cal.IsGCal)
                    {
                        sources.Add(new { googleCalendarApiKey = cal.GoogleAPIKey, googleCalendarId = cal.GCalFeedUrl });
                    }
                    else
                    {
                        sources.Add(new EventSource
                        {
                            url = (sourcePrefix ?? String.Empty) + "/umbraco/EventCalendar/ECApi/CalendarEvents/",
                            data = new { id = cal.Id }
                        });
                    }
                }
            }

            return sources;
        }

        private List<EventsOverviewModel> GetNormalEvents(int id,string culture, DateTime start, DateTime end)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            DateTime startDate = start;
            DateTime endDate = end;

            //Handle normal events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();
            var calendar = CalendarService.GetCalendarById(id);
            var normal_events = EventService.GetAllEvents();
            foreach (var ne in normal_events.Where(x => x.Start <= endDate && x.End >= startDate))
            {
                List<EventDescription> descriptions = db.Query<EventDescription>("SELECT * FROM ec_eventdescriptions WHERE eventid = @0 AND calendarid = @1 AND type = @2", ne.Id, ne.calendarId, (int)EventType.Normal).ToList();
                EventDescription currentDescription = descriptions.SingleOrDefault(x => x.CultureCode.ToLower() == culture);                
                string description = String.Empty;
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                if (null != currentDescription && null != currentDescription.Content) {
                    description = rx.Replace(currentDescription.Content, "");
                    description = description.Substring(0, (description.Length > 150) ? 150 : description.Length) + "...";
                }

                events.Add(
                    new EventsOverviewModel()
                    {
                        type = EventType.Normal,
                        title = ne.Title,
                        allDay = ne.AllDay,
                        description = description,
                        end = ne.End,
                        start = ne.Start,
                        id = ne.Id,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        textColor = !String.IsNullOrEmpty(calendar.TextColor) ? calendar.TextColor : "",
                        categories = ne.Categories,
                        calendar = ne.calendarId
                    });
            }
            return events;
        }

        private List<EventsOverviewModel> GetRecurringEvents(int id, string culture, DateTime start, DateTime end)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            //Handle recurring events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();

            DateTime startDate = start;
            DateTime endDate = end;

            DateRange range = new DateRange();
            range.StartDateTime = startDate;
            range.EndDateTime = endDate;

            var calendar = CalendarService.GetCalendarById(id);
            var recurring_events = RecurringEventService.GetAllEvents().Where(x => x.calendarId == id);
            
            foreach (var e in recurring_events)
            {
                RangeInYear rangeInYear = null;

                if(e.range_start != 0 && e.range_end != 0){
                    rangeInYear = new RangeInYear(){
                        StartMonth = e.range_start,
                        EndMonth = e.range_end
                    };
                }

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event() {
                    Title = e.Title,
                    ID = e.Id,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.Frequency,
                };

                if (rangeInYear != null)
                {
                    tmp_event.RangeInYear = rangeInYear;
                }

                foreach (var day in e.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in e.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                var schedule = new Schedule(tmp_event, e.Exceptions.Select(x => x.Date.Value).ToList());

                foreach (var tmp in schedule.Occurrences(range))
                {
                    List<EventDescription> descriptions = db.Query<EventDescription>("SELECT * FROM ec_eventdescriptions WHERE eventid = @0 AND calendarid = @1 AND type = @2", e.Id, e.calendarId, (int)EventType.Recurring).ToList();
                    EventDescription currentDescription = descriptions.SingleOrDefault(x => x.CultureCode.ToLower() == culture);
                    string description = String.Empty;
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");

                    if (null != currentDescription && null != currentDescription.Content)
                    {
                        description = rx.Replace(currentDescription.Content, "");
                        description = description.Substring(0, (description.Length > 150) ? 150 : description.Length) + "...";
                    }

                    events.Add(new EventsOverviewModel()
                    {
                        title = e.Title,
                        id = e.Id,
                        allDay = e.AllDay,
                        description = description,
                        start = new DateTime(tmp.Year,tmp.Month,tmp.Day,e.Start.Hour,e.Start.Minute,0),
                        end = new DateTime(tmp.Year, tmp.Month, tmp.Day, e.End.Hour, e.End.Minute, 0),
                        type = EventType.Recurring,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        textColor = !String.IsNullOrEmpty(calendar.TextColor) ? calendar.TextColor : "",
                        categories = e.Categories,
                        calendar = e.calendarId
                    });
                }
            }
            return events;
        }
    }
}