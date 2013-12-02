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
    public class REventApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public RecurringEvent PostSave(RecurringEvent nevent)
        {
            if (nevent.Id > 0)
                DatabaseContext.Database.Update(nevent);
            else
                DatabaseContext.Database.Save(nevent);

            return nevent;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<RecurringEvent>(id);
        }

        public RecurringEvent GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents").Where<RecurringEvent>(x => x.Id == id);

            return db.Fetch<RecurringEvent>(query).FirstOrDefault();
        }

        public IEnumerable<RecurringEvent> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_recevents");

            return db.Fetch<RecurringEvent>(query);
        }
    }
}
