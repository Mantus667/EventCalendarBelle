using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EventCalendarBelle.Controller;
using EventCalendarBelle.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace EventCalendarBelle.Services
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
            return db.Delete<ECalendar>(id);
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

            db.Save(calendar);

            //Update usersettings and add the newly created calendar to the allowed calendar
            var ctrl = new UserApiController();
            var usettings = ctrl.GetById(creatorId);
            if (!String.IsNullOrEmpty(usettings.Calendar))
            {
                usettings.Calendar += "," + calendar.Id;
            }
            else
            {
                usettings.Calendar = calendar.Id.ToString();
            }
            ctrl.PostSave(usettings);

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
            var ctrl = new UserApiController();
            var usettings = ctrl.GetById(creatorId);
            if (!String.IsNullOrEmpty(usettings.Calendar))
            {
                usettings.Calendar += "," + calendar.Id;
            }
            else
            {
                usettings.Calendar = calendar.Id.ToString();
            }
            ctrl.PostSave(usettings);

            return calendar;
        }
    }
}
