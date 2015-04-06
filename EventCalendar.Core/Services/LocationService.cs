using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;

namespace EventCalendar.Core.Services
{
    public static class LocationService
    {
        public static int DeleteLocation(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Delete<EventLocation>(id);
        }

        public static EventLocation GetLocation(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations").Where<EventLocation>(x => x.Id == id);

            return db.Fetch<EventLocation>(query).FirstOrDefault();
        }

        public static IEnumerable<EventLocation> GetAllLocations()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations");

            return db.Fetch<EventLocation>(query);
        }

        public static EventLocation UpdateLocation(EventLocation location)
        {
            ApplicationContext.Current.DatabaseContext.Database.Update(location);
            return location;
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

            db.Save(location);

            //Update usersettings and add the newly created calendar to the allowed calendar
            SecurityService.AddLocationToUser(creator, location.Id);

            var args2 = new LocationCreatedEventArgs { Location = location };
            OnCreated(args2);

            return location;
        }

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
        #endregion

        #region EventHandler
        public static event EventHandler<LocationCreatingEventArgs> Creating;
        public static event EventHandler<LocationCreatedEventArgs> Created;
        #endregion
    }
}
