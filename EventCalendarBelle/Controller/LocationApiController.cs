using EventCalendarBelle.Models;
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
                DatabaseContext.Database.Update(location);
            }
            else
            {
                DatabaseContext.Database.Save(location);

                //Update usersettings and add the newly created location to the allowed locations
                var ctrl = new UserApiController();
                var usettings = ctrl.GetById(Security.GetUserId());
                if (!String.IsNullOrEmpty(usettings.Locations))
                {
                    usettings.Locations += "," + location.Id;
                }
                else
                {
                    usettings.Locations = location.Id.ToString();
                }
                ctrl.PostSave(usettings);
            }

            return location;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<EventLocation>(id);
        }

        public EventLocation GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations").Where<EventLocation>(x => x.Id == id);

            return db.Fetch<EventLocation>(query).FirstOrDefault();
        }

        public IEnumerable<EventLocation> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_locations");

            return db.Fetch<EventLocation>(query);
        }
    }
}
