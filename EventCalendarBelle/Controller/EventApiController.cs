using EventCalendar.Core.Models;
using EventCalendar.Core.Services;
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
    public class EventApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public Event PostSave(Event nevent)
        {
            if (nevent.Id > 0)
            {
                //Update the event
                return EventService.UpdateEvent(nevent);
            }
            else
            {
                //Insert the event
                return EventService.CreateEvent(nevent);
            }
        }

        public int DeleteById(int id)
        {
            return EventService.DeleteEvent(id);
        }

        public Event GetById(int id)
        {
            return EventService.GetEvent(id);
        }

        public IEnumerable<Event> GetAll()
        {
            return EventService.GetAllEvents();
        }
    }
}
