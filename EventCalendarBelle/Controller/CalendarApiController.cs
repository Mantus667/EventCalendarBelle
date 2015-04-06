using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using EventCalendar.Core.Models;
using Umbraco.Core.Persistence;
using Newtonsoft.Json;
using System.Web.Http;
using EventCalendar.Core.Services;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class CalendarApiController : UmbracoAuthorizedJsonController
    {
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

            events = ctrl.GetCalendarEvents(DateTime.Now, DateTime.Now.AddYears(1), id).ToList();
            if (forward)
            {
                events = ctrl.GetCalendarEvents(DateTime.Now, DateTime.Now.AddYears(1), id).ToList();
                events = events.OrderBy(x => x.start).ToList();
            }
            else
            {
                events = ctrl.GetCalendarEvents(DateTime.Now, DateTime.Now.AddYears(-1), id).ToList();
                events = events.OrderByDescending(x => x.start).ToList();
            }
            if (quantity != 0)
            {
                return events.Take(quantity);
            }
            return events;
        }
    }
}
