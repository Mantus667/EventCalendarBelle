using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using EventCalendar.Core.Models;
using Umbraco.Core.Persistence;
using Newtonsoft.Json;
using System.Web.Http;
using EventCalendar.Core.Services;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class CalendarApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public ECalendar PostSave(ECalendar calendar)
        {
            var db = UmbracoContext.Application.DatabaseContext.Database;
            if (calendar.Id > 0)
            {
                return CalendarService.UpdateCalendar(calendar);
            }
            else
            {
                return CalendarService.CreateCalendar(calendar, Security.GetUserId());
            }
        }

        public int DeleteById(int id)
        {
            return CalendarService.DeleteCalendarById(id);
        }

        public ECalendar GetById(int id)
        {
            return CalendarService.GetCalendarById(id);
        }

        public IEnumerable<ECalendar> GetAll()
        {
            return CalendarService.GetAllCalendar();
        }

        public PagedCalendarResult GetPaged(int itemsPerPage, int pageNumber, string sortColumn,
            string sortOrder, string searchTerm)
        {
            return CalendarService.GetPagedCalendar(itemsPerPage, pageNumber, sortColumn, sortOrder, searchTerm);
        }
    }
}
