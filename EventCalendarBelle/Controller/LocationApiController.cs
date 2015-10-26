using EventCalendar.Core.Services;
using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class LocationApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public EventLocation PostSave(EventLocation location)
        {
            if (location.Id > 0)
            {
                return LocationService.UpdateLocation(location);
            }
            else
            {
                return LocationService.CreateLocation(location, Security.GetUserId());
            }
        }

        public bool DeleteById(int id)
        {
            return LocationService.DeleteLocation(id);
        }

        public EventLocation GetById(int id)
        {
            return LocationService.GetLocation(id);
        }

        public IEnumerable<EventLocation> GetAll()
        {
            return LocationService.GetAllLocations();
        }

        public PagedLocationsResult GetPaged(int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            var items = new List<EventLocation>();
            var db = DatabaseContext.Database;

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
                           property.GetCustomAttributes(typeof(ColumnAttribute),false);

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
    }
}
