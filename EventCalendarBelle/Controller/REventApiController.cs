using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class REventApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public RecurringEvent PostSave(RecurringEvent nevent)
        {
            var ctrl = new DescriptionApiController();

            if (nevent.Id > 0)
            {
                DatabaseContext.Database.Update(nevent);

                //Save the event descriptions                
                foreach (var desc in nevent.descriptions)
                {
                    ctrl.PostSave(desc);
                }
            }
            else
            {
                DatabaseContext.Database.Save(nevent);
            }

            return nevent;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<RecurringEvent>(id);
        }

        public RecurringEvent GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEvent>(x => x.Id == id);

            RecurringEvent current = db.Fetch<RecurringEvent>(query).FirstOrDefault();

            if (current != null)
            {
                var query2 = new Sql().Select("*").From("ec_eventdescriptions").Where<EventDescription>(x => x.CalendarId == current.calendarId && x.EventId == current.Id && x.Type == (int)EventType.Recurring);
                current.descriptions = db.Fetch<EventDescription>(query2);

                var ls = Services.LocalizationService;
                foreach (var lang in ls.GetAllLanguages())
                {
                    if (current.descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                    {
                        current.descriptions.Add(new EventDescription() { CalendarId = current.calendarId, EventId = current.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Recurring });
                    }
                }
            }

            return current;

            //return db.Fetch<RecurringEvent>(query).FirstOrDefault();
        }

        public IEnumerable<RecurringEvent> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents");

            return db.Fetch<RecurringEvent>(query);
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int,string>> GetDayOfWeekValues()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.DayOfWeekEnum))
                .Cast<ScheduleWidget.Enums.DayOfWeekEnum>()
                .Select(t => new KeyValuePair<int,string>((int)t,t.ToString()));
            return pairs;
            //return Enum.GetValues(typeof(ScheduleWidget.Enums.DayOfWeekEnum));
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> GetFrequencyTypes()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum))
                .Cast<ScheduleWidget.Enums.FrequencyTypeEnum>()
                .Select(t => new KeyValuePair<int, string>((int)t, t.ToString()));
            return pairs;
            //return Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum));
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> GetMonthlyIntervalValues()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum))
                .Cast<ScheduleWidget.Enums.MonthlyIntervalEnum>()
                .Select(t => new KeyValuePair<int, string>((int)t, t.ToString()));
            return pairs;
            //return Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum));
        }
    }
}
