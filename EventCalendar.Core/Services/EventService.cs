using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCalendar.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;

namespace EventCalendar.Core.Services
{
    public static class EventService
    {
        public static Event CreateEvent(Event e)
        {
            var args = new EventCreatingEventArgs { Event = e };
            OnCreating(args);

            if (args.Cancel)
            {
                return e;
            }
            ApplicationContext.Current.DatabaseContext.Database.Save(e);

            //Create the new descriptions for the new event
            e.descriptions = DescriptionService.GetDescriptionsForEvent(e.calendarId, e.Id, EventType.Normal).ToList();

            var ls = ApplicationContext.Current.Services.LocalizationService;
            foreach (var lang in ls.GetAllLanguages())
            {
                if (e.descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                {
                    e.descriptions.Add(new EventDescription() { CalendarId = e.calendarId, EventId = e.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Normal });
                }
            }

            var args2 = new EventCreatedEventArgs { Event = e };
            OnCreated(args2);

            return e;
        }

        public static Event UpdateEvent(Event e)
        {
            ApplicationContext.Current.DatabaseContext.Database.Update(e);

            //Save the event descriptions                
            foreach (var desc in e.descriptions)
            {
                DescriptionService.UpdateDescription(desc);
            }

            return e;
        }

        public static int DeleteEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Delete<Event>(id);
        }

        public static Event GetEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events").Where<Event>(x => x.Id == id);

            Event current = db.Fetch<Event>(query).FirstOrDefault();

            if (current != null)
            {
                current.descriptions = DescriptionService.GetDescriptionsForEvent(current.calendarId, current.Id, EventType.Normal).ToList();

                var ls = ApplicationContext.Current.Services.LocalizationService;
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

        public static IEnumerable<Event> GetAllEvents()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events");

            return db.Fetch<Event>(query);
        }

        #region EventHandler Delegates
        public static void OnCreating(EventCreatingEventArgs e)
        {
            EventHandler<EventCreatingEventArgs> handler = Creating;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnCreated(EventCreatedEventArgs e)
        {
            EventHandler<EventCreatedEventArgs> handler = Created;
            if (handler != null)
            {
                handler(null, e);
            }
        }
        #endregion

        #region EventHandler
        public static event EventHandler<EventCreatingEventArgs> Creating;
        public static event EventHandler<EventCreatedEventArgs> Created;
        #endregion
    }
}
