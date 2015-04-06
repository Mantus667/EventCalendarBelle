using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace EventCalendar.Core.Services
{
    public static class SecurityService
    {
        public static UserSettings GetSecuritySettingsByUserId(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_usettings").Where<UserSettings>(x => x.UserId == id);

            var result = db.Fetch<UserSettings>(query).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return new UserSettings() { Id = 0, UserId = id, CanDeleteLocations = false, CanDeleteEvents = false, CanCreateCalendar = false, CanCreateEvents = false, CanCreateLocations = false, CanDeleteCalendar = false, Calendar = "" };
        }

        public static void AddCalendarToUser(int user_id, int calendar_id)
        {
            var settings = GetSecuritySettingsByUserId(user_id);
            if (!String.IsNullOrEmpty(settings.Calendar))
            {
                settings.Calendar += "," + calendar_id;
            }
            else
            {
                settings.Calendar = calendar_id.ToString();
            }
            UpdateSettings(settings);
        }

        public static void AddLocationToUser(int user, int location)
        {
            var settings = GetSecuritySettingsByUserId(user);
            if (!String.IsNullOrEmpty(settings.Locations))
            {
                settings.Locations += "," + location;
            }
            else
            {
                settings.Locations = location.ToString();
            }
            UpdateSettings(settings);
        }

        public static void UpdateSettings(UserSettings settings)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.Update(settings);
        }
    }
}
