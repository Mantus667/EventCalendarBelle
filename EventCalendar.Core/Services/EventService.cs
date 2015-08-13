using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCalendar.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;
using AutoMapper;
using EventCalendar.Core.Dto;

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
            var dto = Mapper.Map<EventDto>(e);
            ApplicationContext.Current.DatabaseContext.Database.Save(dto);

            //Create the new descriptions for the new event
            e.Descriptions = DescriptionService.GetDescriptionsForEvent(e.calendarId, e.Id, EventType.Normal).ToList();

            var ls = ApplicationContext.Current.Services.LocalizationService;
            foreach (var lang in ls.GetAllLanguages())
            {
                if (e.Descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                {
                    e.Descriptions.Add(new EventDescription() { CalendarId = e.calendarId, EventId = e.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Normal });
                }
            }

            var args2 = new EventCreatedEventArgs { Event = e };
            OnCreated(args2);

            return e;
        }

        public static Event UpdateEvent(Event e)
        {
            var dto = Mapper.Map<EventDto>(e);
            ApplicationContext.Current.DatabaseContext.Database.Update(dto);

            //Save the event descriptions                
            foreach (var desc in e.Descriptions)
            {
                if (desc.Id > 0)
                {
                    DescriptionService.UpdateDescription(desc);
                }
                else
                {
                    DescriptionService.CreateDescription(desc);
                }
            }

            return e;
        }

        public static int DeleteEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var e = GetEvent(id);
            DescriptionService.GetDescriptionsForEvent(e.calendarId, id, EventType.Normal).ForEach(x => DescriptionService.DeleteDescription(x.Id));

            return db.Delete<EventDto>(id);
        }

        public static Event GetEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events").Where<EventDto>(x => x.Id == id);

            var current = db.Fetch<EventDto>(query).FirstOrDefault();
            
            if (current != null)
            {
                var e = Mapper.Map<Event>(current);
                e.Descriptions = DescriptionService.GetDescriptionsForEvent(current.calendarId, current.Id, EventType.Normal).ToList();

                var ls = ApplicationContext.Current.Services.LocalizationService;
                var langs = ls.GetAllLanguages();
                //Check if a language was removed and remove the description
                var descriptionsForDeletedLangs = e.Descriptions.Where(x => !langs.Any(y => y.CultureInfo.ToString() == x.CultureCode));
                foreach (var desc in descriptionsForDeletedLangs)
                {
                    DescriptionService.DeleteDescription(desc.Id);
                    e.Descriptions.Remove(desc);
                }

                //Foreach language if no description is present
                //Create new description for new language
                foreach (var lang in langs)
                {
                    if (e.Descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                    {
                        e.Descriptions.Add(new EventDescription() { CalendarId = current.calendarId, EventId = current.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Normal });
                    }
                }
                return e;
            }

            return null;
        }

        public static EventDetailsModel GetEventDetails(int id)
        {
            //Fetch Event
            var e = EventService.GetEvent(id);

            return Mapper.Map<EventDetailsModel>(e);
        }

        public static IEnumerable<Event> GetAllEvents()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events");

            var events = db.Fetch<EventDto>(query);
            return Mapper.Map<IEnumerable<Event>>(events);
        }

        public static IEnumerable<Event> GetEventsForCalendar(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_events").Where<EventDto>(x => x.calendarId == id);

            var events = db.Fetch<EventDto>(query);
            return Mapper.Map<IEnumerable<Event>>(events);
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
