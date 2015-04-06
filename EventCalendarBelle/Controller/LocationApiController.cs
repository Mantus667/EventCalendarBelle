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

        public int DeleteById(int id)
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
    }
}
