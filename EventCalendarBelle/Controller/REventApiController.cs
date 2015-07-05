using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            nevent.day = string.Join(",", nevent.days.ToArray());
            nevent.monthly_interval = string.Join(",", nevent.intervals.ToArray());

            if (nevent.Id > 0)
            {
                return RecurringEventService.UpdateEvent(nevent);
            }
            else
            {
                return RecurringEventService.CreateEvent(nevent);
            }
        }

        public int DeleteById(int id)
        {
            return RecurringEventService.DeleteRecurringEvent(id);
        }

        public RecurringEvent GetById(int id)
        {
            return RecurringEventService.GetRecurringEvent(id);
        }

        public IEnumerable<RecurringEvent> GetAll()
        {
            return RecurringEventService.GetAllEvents();
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int,string>> GetDayOfWeekValues()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.DayOfWeekEnum))
                .Cast<ScheduleWidget.Enums.DayOfWeekEnum>()
                .Select(t => new KeyValuePair<int,string>((int)t,t.ToString()));
            return pairs;
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> GetFrequencyTypes()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum))
                .Cast<ScheduleWidget.Enums.FrequencyTypeEnum>()
                .Select(t => new KeyValuePair<int, string>((int)t, t.ToString()));
            return pairs;
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> GetMonthlyIntervalValues()
        {
            var pairs = Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum))
                .Cast<ScheduleWidget.Enums.MonthlyIntervalEnum>()
                .Select(t => new KeyValuePair<int, string>((int)t, t.ToString()));
            return pairs;
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> GetMonths()
        {
            return Enumerable.Range(1, 12).Select(i => new KeyValuePair<int,string>(i, DateTimeFormatInfo.CurrentInfo.GetMonthName(i)));
        }
    }
}
