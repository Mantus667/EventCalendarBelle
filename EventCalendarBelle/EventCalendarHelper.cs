using EventCalendarBelle.Models;
using EventCalendarBelle.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Manifest;
using Umbraco.Core;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.UI.JavaScript;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.Models;
using Umbraco.Web.WebServices;
using System.Web;
using System.Web.Routing;
using Umbraco.Web;
using Umbraco.Core.Persistence;

namespace EventCalendarBelle
{
    public class EventCalendarHelper
    {
        public static ECalendar GetCalendar(int id)
        {
            if (id == 0)
            {
                return null;
            }
            else
            {
                return ApplicationContext.Current.DatabaseContext.Database.SingleOrDefault<ECalendar>(id);
            }
        }
    }

    public class ServerVariableEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ServerVariablesParser.Parsing += Parsing;
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }

        private void Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null) throw new InvalidOperationException("HttpContext is null");
            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));            

            var mainDictionary = new Dictionary<string, object>();
            mainDictionary.Add("calendarBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<CalendarApiController>(controller => controller.PostSave(null)));
            mainDictionary.Add("locationBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<LocationApiController>(controller => controller.PostSave(null)));
            mainDictionary.Add("eventBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<EventApiController>(controller => controller.PostSave(null)));
            mainDictionary.Add("reventBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<REventApiController>(controller => controller.PostSave(null)));
            mainDictionary.Add("userBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<UserApiController>(controller => controller.PostSave(null)));

            e.Add("eventCalendar", mainDictionary);
        }
    }
}
