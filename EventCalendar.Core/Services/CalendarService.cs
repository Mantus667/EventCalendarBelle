using System;
using System.Collections.Generic;
using System.Linq;
using EventCalendar.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;
using AutoMapper;
using EventCalendar.Core.Dto;
using ScheduleWidget.ScheduledEvents;
using ScheduleWidget.Enums;

namespace EventCalendar.Core.Services
{
    public static class CalendarService
    {        
        /// <summary>
        /// Returns all available calendar
        /// </summary>
        /// <returns>IEnumerable<ECalendar> of all available calendar</returns>
        public static IEnumerable<ECalendar> GetAllCalendar()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_calendars");
            var calendar = db.Fetch<CalendarDto>(query);
            return Mapper.Map<IEnumerable<ECalendar>>(calendar);
        }

        /// <summary>
        /// Returns a calendar by its id
        /// </summary>
        /// <param name="id">The id of the calendar</param>
        /// <returns>The calendar with the specified id</returns>
        public static ECalendar GetCalendarById(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_calendars").Where<CalendarDto>(x => x.Id == id);

            return Mapper.Map<ECalendar>(db.Fetch<CalendarDto>(query).FirstOrDefault());
        }

        /// <summary>
        /// Deletes a calendar with the given id
        /// </summary>
        /// <param name="id">The id of the calendar</param>
        /// <returns>The id of the deleted calendar</returns>
        public static int DeleteCalendarById(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var calendar = GetCalendarById(id);

            var args = new CalendarDeletionEventArgs { Calendar = calendar };
            OnDeleting(args);

            if (args.Cancel)
            {
                return id;
            }

            EventService.GetEventsForCalendar(id).ForEach(x => EventService.DeleteEvent(x.Id));
            RecurringEventService.GetEventsForCalendar(id).ForEach(x => RecurringEventService.DeleteRecurringEvent(x.Id));
            var response = db.Delete<CalendarDto>(id);

            var args2 = new CalendarDeletedEventArgs { Calendar = calendar };
            OnDeleted(args2);

            return response;
        }

        /// <summary>
        /// Creates a new calendar
        /// </summary>
        /// <param name="calendar">The calendar which should be created in the database</param>
        /// <param name="creatorId">The id of the user who created the calendar</param>
        /// <returns>The newly created calendar</returns>
        public static ECalendar CreateCalendar(ECalendar calendar, int creatorId)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var args = new CalendarCreationEventArgs { Calendar = calendar };
            OnCreating(args);

            if (args.Cancel)
            {
                return calendar;
            }

            db.Save(Mapper.Map<CalendarDto>(calendar));

            //Update usersettings and add the newly created calendar to the allowed calendar
            SecurityService.AddCalendarToUser(creatorId, calendar.Id);

            var args2 = new CalendarCreatedEventArgs { Calendar = calendar };
            OnCreated(args2);

            return calendar;
        }

        /// <summary>
        /// Updates a calendar with now values in database
        /// </summary>
        /// <param name="calendar">The calendar to update</param>
        /// <returns>The updated calendar</returns>
        public static ECalendar UpdateCalendar(ECalendar calendar)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Update(Mapper.Map<CalendarDto>(calendar));
            return calendar;
        }

        /// <summary>
        /// Updates a calendar with now values in database
        /// </summary>
        /// <param name="calendar">The calendar to update</param>
        /// <param name="creatorId">The creator id of the calendar</param>
        /// <returns>The updated calendar</returns>
        public static ECalendar UpdateCalendar(ECalendar calendar, int creatorId)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            db.Update(Mapper.Map<CalendarDto>(calendar));

            //Update usersettings and add the newly created calendar to the allowed calendar
            SecurityService.AddCalendarToUser(creatorId, calendar.Id);

            return calendar;
        }

        /// <summary>
        /// Returns all calendar that the specific user is allowed to see
        /// </summary>
        /// <param name="user">The id of the user</param>
        /// <returns>List of calendar</returns>
        public static IEnumerable<ECalendar> GetCalendarForUser(int user)
        {
            var settings = SecurityService.GetSecuritySettingsByUserId(user);
            var calendar = GetAllCalendar();
            return calendar.Where(x => settings.AllowedCalendar.Contains(x.Id.ToString()));
        }

        /// <summary>
        /// Get paged calendar
        /// </summary>
        /// <param name="itemsPerPage">Items per page</param>
        /// <param name="pageNumber">Current page</param>
        /// <param name="sortColumn">Sort column</param>
        /// <param name="sortOrder">Sort order</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Paged calendar result </returns>
        public static PagedCalendarResult GetPagedCalendar(int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var items = new List<ECalendar>();
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var currentType = typeof(CalendarDto);

            var query = new Sql().Select("*").From("ec_calendars");

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

            var p = db.Page<CalendarDto>(pageNumber, itemsPerPage, query);
            var result = new PagedCalendarResult
            {
                TotalPages = p.TotalPages,
                TotalItems = p.TotalItems,
                ItemsPerPage = p.ItemsPerPage,
                CurrentPage = p.CurrentPage,
                Calendar = Mapper.Map<IEnumerable<ECalendar>>(p.Items).ToList()
            };
            return result;
        }

        public static IEnumerable<EventListModel> GetUpcomingEvents(int take = 10)
        {
            var list = new List<EventListModel>();
            var calendar = CalendarService.GetAllCalendar();

            foreach (var cal in calendar)
            {
                list.AddRange(CalendarService.GetUpcomingEventForCalendar(cal.Id, take));
            }

            return list.OrderBy(x => x.Start).Take(take);
        }

        public static IEnumerable<EventListModel> GetUpcomingEventForCalendar(int calendar, int take = 10)
        {
            var list = new List<EventListModel>();

            //Get normal events
            var events = EventService.GetEventsForCalendar(calendar).Where(x => x.Start >= DateTime.Now).OrderBy(x => x.Start).Take(take);
            //Get recurring events
            var recEvents = RecurringEventService.GetEventsForCalendar(calendar);

            if (!events.Any())
            {
                return Enumerable.Empty<EventListModel>();
            }

            DateRange range = new DateRange();
            range.StartDateTime = DateTime.Now;
            range.EndDateTime = DateTime.Now.AddYears(1);

            //Get occurences for recurring Events
            foreach (var e in recEvents)
            {
                RangeInYear rangeInYear = null;

                if (e.range_start != 0 && e.range_end != 0)
                {
                    rangeInYear = new RangeInYear()
                    {
                        StartMonth = e.range_start,
                        EndMonth = e.range_end
                    };
                }

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.Title,
                    ID = e.Id,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.Frequency,
                };

                if (rangeInYear != null)
                {
                    tmp_event.RangeInYear = rangeInYear;
                }

                foreach (var day in e.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in e.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                var schedule = new Schedule(tmp_event, e.Exceptions.Select(x => x.Date.Value).ToList());

                foreach (var tmp in schedule.Occurrences(range))
                {
                    list.Add(new EventListModel()
                    {
                        Title = e.Title,
                        Id = e.Id,
                        Start = new DateTime(tmp.Year, tmp.Month, tmp.Day, e.Start.Hour, e.Start.Minute, 0),
                        CalendarId = e.calendarId,
                        Type = (int)EventType.Recurring
                    });
                }
            }

            list.AddRange(events.Select(x => new EventListModel { Id = x.Id, Title = x.Title, Start = x.Start.Value, CalendarId = x.calendarId, Type = (int)EventType.Normal }));

            return list.OrderBy(x => x.Start).Take(take);
        }

        /// <summary>
        /// Returns a list of the last events
        /// </summary>
        /// <param name="take">Number of events</param>
        /// <returns>List of events</returns>
        public static IEnumerable<EventListModel> GetLatestEvents(int take = 10)
        {
            var list = new List<EventListModel>();
            var calendar = CalendarService.GetAllCalendar();

            foreach (var cal in calendar)
            {
                list.AddRange(CalendarService.GetLatestEventsForCalendar(cal.Id, take));
            }

            return list.OrderByDescending(x => x.Start).Take(take);
        }

        /// <summary>
        /// Returns a list of the last events for a given calendar
        /// </summary>
        /// <param name="calendar">The calendar to fetch events from</param>
        /// <param name="take">Number of events to show</param>
        /// <returns>List of events</returns>
        public static IEnumerable<EventListModel> GetLatestEventsForCalendar(int calendar, int take = 10)
        {
            var list = new List<EventListModel>();

            //Get the latest normal events for calendar
            var events = EventService.GetEventsForCalendar(calendar).Where(x => x.Start <= DateTime.Now).OrderByDescending(x => x.Start).Take(take);
            var recEvents = RecurringEventService.GetEventsForCalendar(calendar);

            if (!events.Any())
            {
                return Enumerable.Empty<EventListModel>();
            }

            DateRange range = new DateRange();
            range.StartDateTime = events.Last().Start.Value;
            range.EndDateTime = DateTime.Now;

            //Get occurences for recurring Events
            foreach (var e in recEvents)
            {
                RangeInYear rangeInYear = null;

                if (e.range_start != 0 && e.range_end != 0)
                {
                    rangeInYear = new RangeInYear()
                    {
                        StartMonth = e.range_start,
                        EndMonth = e.range_end
                    };
                }

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.Title,
                    ID = e.Id,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.Frequency,
                };

                if (rangeInYear != null)
                {
                    tmp_event.RangeInYear = rangeInYear;
                }

                foreach (var day in e.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in e.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                var schedule = new Schedule(tmp_event, e.Exceptions.Select(x => x.Date.Value).ToList());

                foreach (var tmp in schedule.Occurrences(range))
                {
                    list.Add(new EventListModel()
                    {
                        Title = e.Title,
                        Id = e.Id,
                        Start = new DateTime(tmp.Year, tmp.Month, tmp.Day, e.Start.Hour, e.Start.Minute, 0),
                        CalendarId = e.calendarId,
                        Type = (int)EventType.Recurring
                    });
                }
            }

            list.AddRange(events.Select(x => new EventListModel { Id = x.Id, Title = x.Title, Start = x.Start.Value, CalendarId = x.calendarId, Type = (int)EventType.Normal }));

            return list.OrderByDescending(x => x.Start).Take(take);
        }

        #region EventHandler Delegates
        public static void OnCreating(CalendarCreationEventArgs e)
        {
            EventHandler<CalendarCreationEventArgs> handler = Creating;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnCreated(CalendarCreatedEventArgs e)
        {
            EventHandler<CalendarCreatedEventArgs> handler = Created;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnDeleting(CalendarDeletionEventArgs e)
        {
            EventHandler<CalendarDeletionEventArgs> handler = Deleting;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnDeleted(CalendarDeletedEventArgs e)
        {
            EventHandler<CalendarDeletedEventArgs> handler = Deleted;
            if (handler != null)
            {
                handler(null, e);
            }
        }
        #endregion

        #region EventHandler
        public static event EventHandler<CalendarCreationEventArgs> Creating;
        public static event EventHandler<CalendarCreatedEventArgs> Created;
        public static event EventHandler<CalendarDeletionEventArgs> Deleting;
        public static event EventHandler<CalendarDeletedEventArgs> Deleted;
        #endregion
    }
}
