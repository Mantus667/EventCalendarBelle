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
    public class EventApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public Event PostSave(Event nevent)
        {
            var ctrl = new DescriptionApiController();

            if (nevent.Id > 0)
            {
                //Update the event
                DatabaseContext.Database.Update(nevent);

                //Save the event descriptions                
                foreach (var desc in nevent.descriptions)
                {
                    ctrl.PostSave(desc);
                }
            }
            else
            {
                //Insert the event
                DatabaseContext.Database.Save(nevent);
            }

            return nevent;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<Event>(id);
        }

        public Event GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events").Where<Event>(x => x.Id == id);
            
            Event current = db.Fetch<Event>(query).FirstOrDefault();

            if (current != null)
            {
                var query2 = new Sql().Select("*").From("ec_eventdescriptions").Where<EventDescription>(x => x.CalendarId == current.calendarId && x.EventId == current.Id && x.Type == (int)EventType.Normal);
                current.descriptions = db.Fetch<EventDescription>(query2);

                var ls = Services.LocalizationService;
                foreach (var lang in ls.GetAllLanguages())
                {
                    if (current.descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                    {
                        current.descriptions.Add(new EventDescription() { CalendarId = current.calendarId, EventId = current.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Normal });
                    }
                }
            }

            return current;
        }

        public IEnumerable<Event> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events");

            return db.Fetch<Event>(query);
        }
    }
}
