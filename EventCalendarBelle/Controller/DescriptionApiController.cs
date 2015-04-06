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
using EventCalendar.Core.Services;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class DescriptionApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public EventDescription PostSave(EventDescription description)
        {
            if (description.Id > 0)
            {
                return DescriptionService.UpdateDescription(description);
            }
            else
            {
                return DescriptionService.CreateDescription(description);
            }
        }

        public int DeleteById(int id)
        {
            return DescriptionService.DeleteDescription(id);
        }

        public EventDescription GetById(int id)
        {
            return DescriptionService.GetDescriptionById(id);
        }

        public IEnumerable<EventDescription> GetAll()
        {
            return DescriptionService.GetAllDescriptions();
        }
    }
}
