using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    class LocalizationApiController : UmbracoAuthorizedJsonController
    {
        public Dictionary<string, string> GetAll()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(HttpContext.Current.Server.MapPath("/App_Plugins/EventCalendar/Langs/" + this.UmbracoUser.Language + ".xml"));
            foreach (XmlNode xmlNode1 in xmlDocument.SelectNodes("language/area"))
            {
                string innerText = xmlNode1.Attributes["alias"].InnerText;
                foreach (XmlNode xmlNode2 in xmlNode1.SelectNodes("key"))
                    dictionary.Add(innerText + "_" + xmlNode2.Attributes["alias"].InnerText, xmlNode2.InnerText);
            }
            return dictionary;
        }
    }
}
