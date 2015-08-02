using EventCalendar.Core.Models;
using EventCalendarBelle.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web.UI.JavaScript;
using System.Web;
using System.Web.Routing;
using Umbraco.Web;
using EventCalendar.Core.Services;

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
                return CalendarService.GetCalendarById(id);
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
            mainDictionary.Add("importBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<ImportApiController>(controller => controller.Import()));
            mainDictionary.Add("descriptionBaseUrl", urlHelper.GetUmbracoApiServiceBaseUrl<DescriptionApiController>(controller => controller.PostSave(null)));

            if (!e.Keys.Contains("eventCalendar"))
            {
                e.Add("eventCalendar", mainDictionary);
            }
        }
    }
}
