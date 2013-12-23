using EventCalendarBelle.Models;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
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
        public IEnumerable<EventsOverviewModel> GetCalendarEvents(int id = 0, int start = 0, int end = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            List<EventsOverviewModel> events = new List<EventsOverviewModel>();

            if (id != 0)
            {
                events.AddRange(this.GetNormalEvents(id));
                events.AddRange(this.GetRecurringEvents(id, start, end));
            }
            else
            {
                var calendar = db.Query<ECalendar>("SELECT * FROM ec_calendars").ToList();
                foreach (var cal in calendar)
                {
                    events.AddRange(this.GetNormalEvents(cal.Id));
                    events.AddRange(this.GetRecurringEvents(cal.Id, start, end));
                }
            }

            return events;
        }

        private List<EventsOverviewModel> GetNormalEvents(int id, int start = 0, int end = 0)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;

            DateTime startDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            startDate = startDate.AddSeconds(start);
            DateTime endDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            endDate = endDate.AddSeconds(end);

            //Handle normal events
            List<EventsOverviewModel> events = new List<EventsOverviewModel>();
            var calendar = db.SingleOrDefault<ECalendar>(id);
            var normal_events = db.Query<EventCalendarBelle.Models.Event>("SELECT * FROM ec_events WHERE calendarId = @0", id).ToList();
            foreach (var ne in normal_events.Where(x => x.start >= startDate && x.end <= endDate))
            {
                events.Add(
                    new EventsOverviewModel()
                    {
                        type = EventType.Normal,
                        title = ne.title,
                        allDay = ne.allDay,
                        //description = ne.description,
                        end = ne.end,
                        start = ne.start,
                        id = ne.Id,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        calendar = ne.calendarId
                    });
            }
            return events;
        }

        private List<EventsOverviewModel> GetRecurringEvents(int id, int start = 0, int end = 0)
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
                    events.Add(new EventsOverviewModel()
                    {
                        title = e.title,
                        id = e.Id,
                        allDay = e.allDay,
                        //description = e.description,
                        start = tmp,
                        type = EventType.Recurring,
                        color = !String.IsNullOrEmpty(calendar.Color) ? calendar.Color : "",
                        calendar = e.calendarId
                    });
                }
            }
            return events;
        }
    }
}
