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
    public class UserApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public UserSettings PostSave(UserSettings user)
        {
            if (user.Id > 0)
                DatabaseContext.Database.Update(user);
            else
                DatabaseContext.Database.Save(user);

            return user;
        }

        public UserSettings GetById(int id)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_usettings").Where<UserSettings>(x => x.UserId == id);

            var result = db.Fetch<UserSettings>(query).FirstOrDefault();
            if(result != null) {
                return result;
            }
            return new UserSettings() { Id = 0, UserId = id, CanDeleteLocations = false, CanDeleteEvents = false, CanCreateCalendar = false, CanCreateEvents = false, CanCreateLocations = false, CanDeleteCalendar = false, Calendar = "" };
        }

        public IEnumerable<UserSettings> GetAll()
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_usettings");

            return db.Fetch<UserSettings>(query);
        }

        public string GetNameOfUser(int id)
        {
            var us = Services.UserService;
            var user = us.GetByProviderKey(id);
            return user.Username;
        }
    }
}
