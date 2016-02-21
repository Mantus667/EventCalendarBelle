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
            DescriptionService.GetDescriptionsForEvent(re.calendarId, id, EventType.Recurring).ForEach(x => DescriptionService.DeleteDescription(x.Id));

            return db.Delete<RecurringEventDto>(id);
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
                var langs = ls.GetAllLanguages();
                //Check if a language was removed and remove the description
                var descriptionsForDeletedLangs = result.Descriptions.Where(x => !langs.Any(y => y.CultureInfo.ToString() == x.CultureCode));
                foreach (var desc in descriptionsForDeletedLangs)
                {
                    DescriptionService.DeleteDescription(desc.Id);
                    result.Descriptions.Remove(desc);
                }

                //Foreach language if no description is present
                //Create new description for new language
                foreach (var lang in langs)
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
            revent.Descriptions = DescriptionService.GetDescriptionsForEvent(revent.calendarId, revent.Id, EventType.Recurring).ToList();

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

        public static IEnumerable<RecurringEvent> GetEventsForLocation(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEventDto>(x => x.locationId == id);

            var events = db.Fetch<RecurringEventDto>(query);

            return Mapper.Map<IEnumerable<RecurringEvent>>(events);
        }

        public static PagedREventsResult GetPagedEvents(int calendar, int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var currentType = typeof(RecurringEventDto);

            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEventDto>(x => x.calendarId == calendar);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int c = 0;
                foreach (var property in currentType.GetProperties())
                {
                    string before = "WHERE";
                    if (c > 0)
                    {
                        before = "OR";
                    }

                    var columnAttri =
                           property.GetCustomAttributes(typeof(ColumnAttribute), false);

                    var columnName = property.Name;
                    if (columnAttri.Any())
                    {
                        columnName = ((ColumnAttribute)columnAttri.FirstOrDefault()).Name;
                    }

                    query.Append(before + " [" + columnName + "] like @0", "%" + searchTerm + "%");
                    c++;
                }
            }
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
                query.OrderBy(sortColumn + " " + sortOrder);
            else
            {
                query.OrderBy("id asc");
            }

            var p = db.Page<RecurringEventDto>(pageNumber, itemsPerPage, query);
            var result = new PagedREventsResult
            {
                TotalPages = p.TotalPages,
                TotalItems = p.TotalItems,
                ItemsPerPage = p.ItemsPerPage,
                CurrentPage = p.CurrentPage,
                Events = Mapper.Map<IEnumerable<RecurringEvent>>(p.Items).ToList()
            };
            return result;
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
