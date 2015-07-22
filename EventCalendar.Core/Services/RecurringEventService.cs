using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.Models;
using EventCalendar.Core.EventArgs;
using EventCalendar.Core.Dto;
using AutoMapper;

namespace EventCalendar.Core.Services
{
    public static class RecurringEventService
    {
        public static int DeleteRecurringEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var re = GetRecurringEvent(id);
            DescriptionService.GetDescriptionsForEvent(re.calendarId, id, EventType.Normal).ForEach(x => DescriptionService.DeleteDescription(x.Id));

            return db.Delete<RecurringEvent>(id);
        }

        public static IEnumerable<RecurringEvent> GetAllEvents()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents");

            var events = db.Fetch<RecurringEventDto>(query);

            return Mapper.Map<IEnumerable<RecurringEvent>>(events);
        }

        public static IEnumerable<RecurringEvent> GetEventsForCalendar(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEventDto>(x => x.calendarId == id);

            var events = db.Fetch<RecurringEventDto>(query);

            return Mapper.Map<IEnumerable<RecurringEvent>>(events);
        }

        public static RecurringEvent GetRecurringEvent(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEventDto>(x => x.Id == id);

            var current = db.Fetch<RecurringEventDto>(query).FirstOrDefault();

            if (current != null)
            {
                var result = Mapper.Map<RecurringEvent>(current);
                result.Descriptions = DescriptionService.GetDescriptionsForEvent(current.calendarId, current.Id, EventType.Recurring).ToList();

                var ls = ApplicationContext.Current.Services.LocalizationService;
                foreach (var lang in ls.GetAllLanguages())
                {
                    if (result.Descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                    {
                        result.Descriptions.Add(new EventDescription() { CalendarId = current.calendarId, EventId = current.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Recurring });
                    }
                }
                return result;
            }

            return null;
        }

        public static RecurringEvent UpdateEvent(RecurringEvent revent)
        {
            var dto = Mapper.Map<RecurringEventDto>(revent);

            ApplicationContext.Current.DatabaseContext.Database.Update(dto);

            //Save the event descriptions                
            foreach (var desc in revent.Descriptions)
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

            //Get all exceptions for this event
            var exceptions = DateExceptionService.GetDateExceptionsForRecurringEvent(revent.Id);
            //Get deleted exceptions from frontend and delete them from database
            var deleted = exceptions.Where(x => !revent.Exceptions.Contains(x));
            deleted.ForEach(x => DateExceptionService.DeleteDateException(x.Id));
            //Save/Update the date exceptions
            foreach (var de in revent.Exceptions)
            {
                if (de.Id > 0)
                {
                    DateExceptionService.UpdateDateException(de);
                }
                else
                {
                    DateExceptionService.CreateDateException(de);
                }
            }

            return Mapper.Map<RecurringEvent>(dto);
        }

        public static RecurringEvent CreateEvent(RecurringEvent revent)
        {
            var args = new RecurringEventCreatingEventArgs { RecurringEvent = revent };
            OnCreating(args);

            if (args.Cancel)
            {
                return revent;
            }

            var dto = Mapper.Map<RecurringEventDto>(revent);

            ApplicationContext.Current.DatabaseContext.Database.Save(dto);

            //Create the new descriptions for the new event
            revent.Descriptions = DescriptionService.GetDescriptionsForEvent(revent.calendarId, revent.Id, EventType.Normal).ToList();

            var ls = ApplicationContext.Current.Services.LocalizationService;
            foreach (var lang in ls.GetAllLanguages())
            {
                if (revent.Descriptions.SingleOrDefault(x => x.CultureCode == lang.CultureInfo.ToString()) == null)
                {
                    revent.Descriptions.Add(new EventDescription() { CalendarId = revent.calendarId, EventId = revent.Id, CultureCode = lang.CultureInfo.ToString(), Id = 0, Type = (int)EventType.Recurring });
                }
            }

            var args2 = new RecurringEventCreatedEventArgs { RecurringEvent = revent };
            OnCreated(args2);

            return Mapper.Map<RecurringEvent>(dto);
        }

        public static EventDetailsModel GetEventDetails(int id)
        {
            var e = RecurringEventService.GetRecurringEvent(id);
            return Mapper.Map<EventDetailsModel>(e);
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
