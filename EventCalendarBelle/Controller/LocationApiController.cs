using EventCalendar.Core.Services;
using EventCalendar.Core.Models;
using System.Collections.Generic;
using System.Web.Http;
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
            return LocationService.GetPagedLocations(itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm);
        }
    }
}
