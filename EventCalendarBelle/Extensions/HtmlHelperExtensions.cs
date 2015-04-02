using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EventCalendarBelle.Models;
using Newtonsoft.Json;

namespace EventCalendarBelle.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString RenderUpcomingEventsList(this HtmlHelper helper, string value)
        {
            dynamic options = JsonConvert.DeserializeObject(value);

            var data = new GridEditorListValueModel {
                Title = (string)options.title,
                Teaser = (bool)options.teaser,
                Quantity = (int)options.count,
                CalendarId = (int)options.calendar,
                Forward = true
            };

            return helper.Action("RenderGridEditorList", "ECSurface", new { area = "EventCalendar", data = data });
        }
    }
}
