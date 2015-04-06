using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.Models;
using EventCalendar.Core.EventArgs;

namespace EventCalendar.Core.Services
{
    public static class RecurringEventService
    {
        public static int DeleteRecurringEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Delete<RecurringEvent>(id);
        }

        public static IEnumerable<RecurringEvent> GetAllEvents()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents");

            return db.Fetch<RecurringEvent>(query);
        }

        public static RecurringEvent GetRecurringEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEvent>(x => x.Id == id);

            RecurringEvent current = db.Fetch<RecurringEvent>(query).FirstOrDefault();

            if (current != null)
            {
                current.descriptions = DescriptionService.GetDescriptionsForEvent(current.calendarId, current.Id, EventType.Recurring).ToList();

                var ls = ApplicationContext.Current.Services.LocalizationService;
                foreach (var lang in ls.GetAllLanguages())
                {
                    if (current.descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                    {
                        current.descriptions.Add(new EventDescription() { CalendarId = current.calendarId, EventId = current.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Recurring });
                    }
                }
            }

            return current;
        }

        public static RecurringEvent UpdateEvent(RecurringEvent revent)
        {
            ApplicationContext.Current.DatabaseContext.Database.Update(revent);

            //Save the event descriptions                
            foreach (var desc in revent.descriptions)
            {
                DescriptionService.UpdateDescription(desc);
            }

            return revent;
        }

        public static RecurringEvent CreateEvent(RecurringEvent revent)
        {
            var args = new RecurringEventCreatingEventArgs { RecurringEvent = revent };
            OnCreating(args);

            if (args.Cancel)
            {
                return revent;
            }

            ApplicationContext.Current.DatabaseContext.Database.Save(revent);

            //Create the new descriptions for the new event
            revent.descriptions = DescriptionService.GetDescriptionsForEvent(revent.calendarId, revent.Id, EventType.Normal).ToList();

            var ls = ApplicationContext.Current.Services.LocalizationService;
            foreach (var lang in ls.GetAllLanguages())
            {
                if (revent.descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                {
                    revent.descriptions.Add(new EventDescription() { CalendarId = revent.calendarId, EventId = revent.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Recurring });
                }
            }

            var args2 = new RecurringEventCreatedEventArgs { RecurringEvent = revent };
            OnCreated(args2);

            return revent;
        }

        #region EventHandler Delegates
        public static void OnCreating(RecurringEventCreatingEventArgs e)
        {
            EventHandler<RecurringEventCreatingEventArgs> handler = Creating;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnCreated(RecurringEventCreatedEventArgs e)
        {
            EventHandler<RecurringEventCreatedEventArgs> handler = Created;
            if (handler != null)
            {
                handler(null, e);
            }
        }
        #endregion

        #region EventHandler
        public static event EventHandler<RecurringEventCreatingEventArgs> Creating;
        public static event EventHandler<RecurringEventCreatedEventArgs> Created;
        #endregion
    }
}
