using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class LocalizationApiController : UmbracoAuthorizedJsonController
    {
        [HttpGet]
        public string GetCurrentDateFormat(string locale = "en-US")
        {
            var culture = new CultureInfo(locale);
            return culture.DateTimeFormat.LongDatePattern;
        }
    }
}
