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
    public class DescriptionApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public EventDescription PostSave(EventDescription description)
        {
            if (description.Id > 0)
                DatabaseContext.Database.Update(description);
            else
                DatabaseContext.Database.Save(description);

            return description;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<EventDescription>(id);
        }

        public EventDescription GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_eventdescriptions").Where<EventDescription>(x => x.Id == id);

            return db.Fetch<EventDescription>(query).FirstOrDefault(); ;
        }

        public IEnumerable<EventDescription> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_eventdescriptions");

            return db.Fetch<EventDescription>(query);
        }
    }
}
