using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using EventCalendar.Core.EventArgs;

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

            var deletedID = db.Delete<EventLocation>(id);

            var args2 = new LocationDeletedEventArgs { Location = location };
            OnDeleted(args2);

            return deletedID != 0;
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

        public static IEnumerable<EventLocation> GetLocationsForUser(int user)
        {
            var settings = SecurityService.GetSecuritySettingsByUserId(user);
            var locations = GetAllLocations();
            return locations.Where(x => settings.Locations.Contains(x.Id.ToString()));
        }

        public static PagedLocationsResult GetPagedLocations(int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var items = new List<EventLocation>();
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var currentType = typeof(EventLocation);

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

            var p = db.Page<EventLocation>(pageNumber, itemsPerPage, query);
            var result = new PagedLocationsResult
            {
                TotalPages = p.TotalPages,
                TotalItems = p.TotalItems,
                ItemsPerPage = p.ItemsPerPage,
                CurrentPage = p.CurrentPage,
                Locations = p.Items
            };
            return result;
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
