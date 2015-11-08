using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
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

        public IEnumerable<RecurringEvent> GetForCalendar(int id)
        {
            return RecurringEventService.GetEventsForCalendar(id);
        }

        public PagedREventsResult GetPaged(int calendar, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            return RecurringEventService.GetPagedEvents(calendar, itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm);
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
