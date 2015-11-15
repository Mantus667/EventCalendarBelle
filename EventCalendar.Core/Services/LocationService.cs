using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;
using ScheduleWidget.ScheduledEvents;
using ScheduleWidget.Enums;
using EventCalendar.Core.Dto;
using AutoMapper;

namespace EventCalendar.Core.Services
{
    public static class LocationService
    {
        public static bool DeleteLocation(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var location = GetLocation(id);

            var args = new LocationDeletionEventArgs { Location = location };
            OnDeleting(args);

            if (args.Cancel || CanLocationBeDeleted(id) == false)
            {
                return false;
            }

            var deletedID = db.Delete<EventLocationDto>(id);

            var args2 = new LocationDeletedEventArgs { Location = location };
            OnDeleted(args2);

            return deletedID != 0;
        }

        public static EventLocation GetLocation(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations").Where<EventLocationDto>(x => x.Id == id);

            var dto =  db.Fetch<EventLocationDto>(query).FirstOrDefault();
            return Mapper.Map<EventLocation>(dto);
        }

        public static IEnumerable<EventLocation> GetAllLocations()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations");

            var dtos =  db.Fetch<EventLocationDto>(query);
            return Mapper.Map<IEnumerable<EventLocation>>(dtos);
        }

        public static EventLocation UpdateLocation(EventLocation location)
        {
            var dto = Mapper.Map<EventLocationDto>(location);
            ApplicationContext.Current.DatabaseContext.Database.Update(dto);
            return Mapper.Map<EventLocation>(dto);
        }

        public static EventLocation CreateLocation(EventLocation location, int creator)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var args = new LocationCreatingEventArgs { Location = location };
            OnCreating(args);

            if (args.Cancel)
            {
                return location;
            }
            var dto = Mapper.Map<EventLocationDto>(location);
            db.Save(dto);

            var result = Mapper.Map<EventLocation>(dto);

            //Update usersettings and add the newly created calendar to the allowed calendar
            SecurityService.AddLocationToUser(creator, result.Id);

            var args2 = new LocationCreatedEventArgs { Location = result };
            OnCreated(args2);

            return result;
        }

        /// <summary>
        /// Get locations for a specific user
        /// </summary>
        /// <param name="user">The id of the user</param>
        /// <returns>List of locations for specified user</returns>
        public static IEnumerable<EventLocation> GetLocationsForUser(int user)
        {
            var settings = SecurityService.GetSecuritySettingsByUserId(user);
            var locations = GetAllLocations();
            if (locations == null || !locations.Any() || settings.Locations == null || String.IsNullOrEmpty(settings.Locations))
            {
                return Enumerable.Empty<EventLocation>();
            }
            return locations.Where(x => settings.Locations.Contains(x.Id.ToString()));
        }

        /// <summary>
        /// Get paged locations
        /// </summary>
        /// <param name="itemsPerPage">Locations per page</param>
        /// <param name="pageNumber">Current page</param>
        /// <param name="sortColumn">Sort column</param>
        /// <param name="sortOrder">Sort order</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Paged locations</returns>
        public static PagedLocationsResult GetPagedLocations(int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var items = new List<EventLocation>();
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var currentType = typeof(EventLocationDto);

            var query = new Sql().Select("*").From("ec_locations");

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

            var p = db.Page<EventLocationDto>(pageNumber, itemsPerPage, query);
            var result = new PagedLocationsResult
            {
                TotalPages = p.TotalPages,
                TotalItems = p.TotalItems,
                ItemsPerPage = p.ItemsPerPage,
                CurrentPage = p.CurrentPage,
                Locations = Mapper.Map<IEnumerable<EventLocation>>(p.Items).ToList()
            };
            return result;
        }

        public static IEnumerable<EventListModel> GetUpcomingEventsForLocation(int location, int take = 10)
        {
            var list = new List<EventListModel>();

            //Get normal events
            var events = EventService.GetEventsForLocation(location).Where(x => x.Start >= DateTime.Now).OrderBy(x => x.Start).Take(take);
            //Get recurring events
            var recEvents = RecurringEventService.GetEventsForLocation(location);

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
        /// Returns a list of the last events for a given calendar
        /// </summary>
        /// <param name="calendar">The calendar to fetch events from</param>
        /// <param name="take">Number of events to show</param>
        /// <returns>List of events</returns>
        public static IEnumerable<EventListModel> GetLatestEventsForLocation(int location, int take = 10)
        {
            var list = new List<EventListModel>();

            //Get the latest normal events for calendar
            var events = EventService.GetEventsForLocation(location).Where(x => x.Start <= DateTime.Now).OrderByDescending(x => x.Start).Take(take);
            var recEvents = RecurringEventService.GetEventsForLocation(location);

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

        #region Private Stuff

        private static bool CanLocationBeDeleted(int id){
            return !EventService.GetEventsForLocation(id).Any() && !RecurringEventService.GetEventsForLocation(id).Any();
        }

        #endregion

        #region EventHandler Delegates
        public static void OnCreating(LocationCreatingEventArgs e)
        {
            EventHandler<LocationCreatingEventArgs> handler = Creating;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnCreated(LocationCreatedEventArgs e)
        {
            EventHandler<LocationCreatedEventArgs> handler = Created;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnDeleting(LocationDeletionEventArgs e)
        {
            EventHandler<LocationDeletionEventArgs> handler = Deleting;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        public static void OnDeleted(LocationDeletedEventArgs e)
        {
            EventHandler<LocationDeletedEventArgs> handler = Deleted;
            if (handler != null)
            {
                handler(null, e);
            }
        }
        #endregion

        #region EventHandler
        public static event EventHandler<LocationCreatingEventArgs> Creating;
        public static event EventHandler<LocationCreatedEventArgs> Created;
        public static event EventHandler<LocationDeletionEventArgs> Deleting;
        public static event EventHandler<LocationDeletedEventArgs> Deleted;
        #endregion
    }
}
