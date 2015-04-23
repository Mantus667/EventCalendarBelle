using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EventCalendar.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;

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

            return db.Fetch<ECalendar>(query);
        }

        /// <summary>
        /// Returns a calendar by its id
        /// </summary>
        /// <param name="id">The id of the calendar</param>
        /// <returns>The calendar with the specified id</returns>
        public static ECalendar GetCalendarById(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_calendars").Where<ECalendar>(x => x.Id == id);

            return db.Fetch<ECalendar>(query).FirstOrDefault();
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
            var response = db.Delete<ECalendar>(id);

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

            db.Save(calendar);

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
            db.Update(calendar);
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

            db.Update(calendar);

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
