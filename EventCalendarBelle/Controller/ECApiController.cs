using EventCalendarBelle.Models;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class ECApiController : UmbracoApiController
    {

        [HttpGet]
        public IEnumerable<EventsOverviewModel> GetCalendarEvents(int id = 0,string culture = "en-us", int start = 0, int end = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            List<EventsOverviewModel> events = new List<EventsOverviewModel>();

            if (id != 0)
            {
                events.AddRange(this.GetNormalEvents(id,culture, start, end));
                events.AddRange(this.GetRecurringEvents(id, culture, start, end));
            }
            else
            {
                var calendar = db.Query<ECalendar>("SELECT * FROM ec_calendars").ToList();
                foreach (var cal in calendar)
                {
                    events.AddRange(this.GetNormalEvents(cal.Id, culture, start, end));
                    events.AddRange(this.GetRecurringEvents(cal.Id, culture, start, end));
                }
            }

            return events;
        }

        private List<EventsOverviewModel> GetNormalEvents(int id,string culture, int start = 0, int end = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            DateTime startDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            startDate = startDate.AddSeconds(start);
            DateTime endDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            endDate = endDate.AddSeconds(end);

            //Handle normal events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();
            var calendar = db.SingleOrDefault<ECalendar>(id);
            var normal_events = db.Fetch<EventCalendarBelle.Models.Event>("SELECT * FROM ec_events WHERE ec_events.calendarId = @0", id).ToList();
            foreach (var ne in normal_events.Where(x => x.start >= startDate && x.end <= endDate))
            {
                List<EventDescription> descriptions = db.Query<EventDescription>("SELECT * FROM ec_eventdescriptions WHERE eventid = @0 AND calendarid = @1", ne.Id, ne.calendarId).ToList();
                EventDescription currentDescription = descriptions.SingleOrDefault(x => x.CultureCode.ToLower() == culture);                
                string description = String.Empty;
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
                if (null != currentDescription) {
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
                        categories = ne.categories,
                        calendar = ne.calendarId
                    });
            }
            return events;
        }

        private List<EventsOverviewModel> GetRecurringEvents(int id, string culture, int start = 0, int end = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            //Handle recurring events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();

            DateTime startDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            startDate = startDate.AddSeconds(start);
            DateTime endDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            endDate = endDate.AddSeconds(end);

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
                    List<EventDescription> descriptions = db.Query<EventDescription>("SELECT * FROM ec_eventdescriptions WHERE eventid = @0 AND calendarid = @1", e.Id, e.calendarId).ToList();
                    EventDescription currentDescription = descriptions.SingleOrDefault(x => x.CultureCode.ToLower() == culture);
                    string description = String.Empty;
                    System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");

                    if (null != currentDescription)
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
                        categories = e.categories,
                        calendar = e.calendarId
                    });
                }
            }
            return events;
        }
    }
}
