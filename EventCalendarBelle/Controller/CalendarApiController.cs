using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using EventCalendarBelle.Models;
using Umbraco.Core.Persistence;
using Newtonsoft.Json;
using System.Web.Http;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class CalendarApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public ECalendar PostSave(ECalendar calendar)
        {
            if (calendar.Id > 0)
            {
                DatabaseContext.Database.Update(calendar);
            }
            else
            {
                DatabaseContext.Database.Save(calendar);

                //Update usersettings and add the newly created calendar to the allowed calendar
                var ctrl = new UserApiController();
                var usettings = ctrl.GetById(Security.GetUserId());
                if(!String.IsNullOrEmpty(usettings.Calendar)) {
                    usettings.Calendar += "," + calendar.Id;
                }
                else
                {
                    usettings.Calendar = calendar.Id.ToString();
                }
                ctrl.PostSave(usettings);
            }

            return calendar;
        }

        public int DeleteById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            return db.Delete<ECalendar>(id);
        }

        public ECalendar GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_calendars").Where<ECalendar>(x => x.Id == id);

            return db.Fetch<ECalendar>(query).FirstOrDefault();
        }

        public IEnumerable<ECalendar> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_calendars");

            return db.Fetch<ECalendar>(query);
        }
    }
}
