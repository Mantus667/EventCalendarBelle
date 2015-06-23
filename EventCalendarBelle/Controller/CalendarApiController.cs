using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using EventCalendar.Core.Models;
using Umbraco.Core.Persistence;
using System.Web.Http;
using EventCalendar.Core.Services;
using System.Web.Configuration;
using ScheduleWidget.ScheduledEvents;
using ScheduleWidget.Enums;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class CalendarApiController : UmbracoAuthorizedJsonController
    {
        public string sourcePrefix { get; private set; }

        public CalendarApiController()
        {
            if (WebConfigurationManager.AppSettings.AllKeys.Contains("EventCalendar:EventSourcePrefix"))
            {
                sourcePrefix = WebConfigurationManager.AppSettings["EventCalendar:EventSourcePrefix"];
            }
        }

        [HttpPost]
        public ECalendar PostSave(ECalendar calendar)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            if (calendar.Id > 0)
            {
                return CalendarService.UpdateCalendar(calendar);
            }
            else
            {
                return CalendarService.CreateCalendar(calendar, Security.GetUserId());
            }
        }

        public int DeleteById(int id)
        {
            return CalendarService.DeleteCalendarById(id);
        }

        public ECalendar GetById(int id)
        {
            return CalendarService.GetCalendarById(id);
        }

        public IEnumerable<ECalendar> GetAll()
        {
            return CalendarService.GetAllCalendar();
        }

        public IEnumerable<EventsOverviewModel> GetEvents(int id,bool forward, int quantity = 0)
        {
            var ctrl = new ECApiController();
            var events = new List<EventsOverviewModel>();

            events = ctrl.CalendarEvents(new EventRange { start = DateTime.Now, end = DateTime.Now.AddYears(1), id = id }).ToList();
            if (forward)
            {
                events = ctrl.CalendarEvents(new EventRange { start = DateTime.Now, end = DateTime.Now.AddYears(1), id = id }).ToList();
                events = events.OrderBy(x => x.start).ToList();
            }
            else
            {
                events = ctrl.CalendarEvents(new EventRange { start = DateTime.Now, end = DateTime.Now.AddYears(-1), id = id }).ToList();
                events = events.OrderByDescending(x => x.start).ToList();
            }
            if (quantity != 0)
            {
                return events.Take(quantity);
            }
            return events;
        }

        public IEnumerable<object> GetCalendarSources(int id = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            List<object> sources = new List<object>();

            //Check if we got an id, otherwise get all calendar
            if (id != 0)
            {
                var query = new Sql().Select("*").From("ec_calendars").Where<ECalendar>(x => x.Id == id);
                var calendar = db.Fetch<ECalendar>(query).FirstOrDefault();

                sources.Add(GetSourceForCalendar(calendar));

            }
            else
            {
                //Get all calendar
                var query = new Sql().Select("*").From("ec_calendars");
                var calendar = db.Fetch<ECalendar>(query);

                foreach (var cal in calendar)
                {
                    sources.Add(GetSourceForCalendar(cal));
                }
            }

            return sources;
        }

        [HttpGet]
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
        public IEnumerable<EventsOverviewModel> CalendarEvents(int id, DateTime start, DateTime end)
        {
            return CalendarEvents(new EventRange { id = id, start = start, end = end });
        }

        private object GetSourceForCalendar(ECalendar calendar)
        {
            if (calendar.IsGCal)
            {
                return new { googleCalendarApiKey = calendar.GoogleAPIKey, googleCalendarId = calendar.GCalFeedUrl };
            }
            else
            {
                return new EventSource
                {
                    url = (sourcePrefix ?? String.Empty) + "/umbraco/backoffice/EventCalendar/CalendarApi/CalendarEvents/?id=" + calendar.Id,
                    type = "GET"
                };
            }
        }

        //TODO: Auf services umstellen
        private List<EventsOverviewModel> GetNormalEvents(int id, string culture, DateTime start, DateTime end)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            DateTime startDate = start;
            DateTime endDate = end;

            //Handle normal events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();
            var calendar = db.SingleOrDefault<ECalendar>(id);
            var normal_events = db.Fetch<EventCalendar.Core.Models.Event>("SELECT * FROM ec_events WHERE ec_events.calendarId = @0", id).ToList();
            foreach (var ne in normal_events.Where(x => x.start <= endDate && x.end >= startDate))
            {
                List<EventDescription> descriptions = db.Query<EventDescription>("SELECT * FROM ec_eventdescriptions WHERE eventid = @0 AND calendarid = @1 AND type = @2", ne.Id, ne.calendarId, (int)EventType.Normal).ToList();
                EventDescription currentDescription = descriptions.SingleOrDefault(x => x.CultureCode.ToLower() == culture);
                string description = String.Empty;
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                if (null != currentDescription && null != currentDescription.Content)
                {
                    description = rx.Replace(currentDescription.Content, "");
                    description = description.Substring(0, (description.Length > 150) ? 150 : description.Length) + "...";
                }

                events.Add(
                    new EventsOverviewModel()
                    {
                        type = EventType.Normal,
                        title = ne.title,
                        allDay = ne.allDay,
                        description = description,
                        end = ne.end,
                        start = ne.start,
                        id = ne.Id,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        textColor = !String.IsNullOrEmpty(calendar.TextColor) ? calendar.TextColor : "",
                        categories = ne.categories,
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

            var calendar = db.SingleOrDefault<ECalendar>(id);
            var recurring_events = db.Query<RecurringEvent>("SELECT * FROM ec_recevents WHERE calendarId = @0 ORDER BY id DESC", id).ToList();
            foreach (var e in recurring_events)
            {
                var schedule = new Schedule(
                    new ScheduleWidget.ScheduledEvents.Event()
                    {
                        Title = e.title,
                        ID = e.Id,
                        DaysOfWeekOptions = (DayOfWeekEnum)e.day,
                        FrequencyTypeOptions = (FrequencyTypeEnum)e.frequency,
                        MonthlyIntervalOptions = (MonthlyIntervalEnum)e.monthly_interval
                    });
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
                        title = e.title,
                        id = e.Id,
                        allDay = e.allDay,
                        description = description,
                        start = tmp,
                        type = EventType.Recurring,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        textColor = !String.IsNullOrEmpty(calendar.TextColor) ? calendar.TextColor : "",
                        categories = e.categories,
                        calendar = e.calendarId
                    });
                }
            }
            return events;
        }
    }
}
