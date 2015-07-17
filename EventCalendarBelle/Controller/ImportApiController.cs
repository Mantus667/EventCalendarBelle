using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using System.IO;
using EventCalendar.Core.Models;
using ScheduleWidget.Enums;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class ImportApiController : UmbracoAuthorizedJsonController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                if (provider.FormData["calendar"] == null || provider.FormData["calendar"] == "undefined")
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                int calendarId = int.Parse(provider.FormData["calendar"]);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    //Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    //Trace.WriteLine("Server file path: " + file.LocalFileName);
                    var collection = iCalendar.LoadFromFile(file.LocalFileName);                    

                    //If calendarId == 0 create a new calendar
                    //else add events to an existing calendar
                    if (calendarId == 0)
                    {
                        ImportEventsToNewCalendar(collection);
                    }
                    else
                    {
                        ImportEventsIntoExistingCalendar(collection, calendarId);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private void ImportEventsIntoExistingCalendar(IICalendarCollection collection, int calendar_id)
        {
            var cctrl = new CalendarApiController();
            var ectrl = new EventApiController();
            var rectrl = new REventApiController();

            //loop through calendar in collection
            foreach (var cal in collection)
            {
                //loop through events in calendar
                foreach (var e in cal.Events)
                {
                    //check if it is an reccuring event
                    if (e.RecurrenceRules.Count > 0)
                    {
                        var revent = new RecurringEvent(){ AllDay = e.IsAllDay, Title = e.Summary, Start = e.Start.Date, End = e.Start.Date, calendarId = calendar_id, Categories = String.Join(",", e.Categories), Id = 0};
                        revent.MonthlyIntervals.Add(((int)MonthlyIntervalEnum.First));
                        RecurrencePattern rp = (RecurrencePattern)e.RecurrenceRules[0];
                        switch (rp.Frequency)
                        {
                            case FrequencyType.Daily:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Daily;
                                    break;
                                }
                            case FrequencyType.Monthly:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Monthly;
                                    break;
                                }
                            case FrequencyType.Weekly:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Weekly;
                                    break;
                                }
                            default:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Monthly;
                                    break;
                                }
                        }
                        switch(e.Start.DayOfWeek) {
                            case DayOfWeek.Monday: {
                                revent.Days.Add((int)DayOfWeekEnum.Mon);
                                break;
                            }
                            case DayOfWeek.Tuesday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Tue);
                                    break;
                                }
                            case DayOfWeek.Wednesday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Wed);
                                    break;
                                }
                            case DayOfWeek.Thursday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Thu);
                                    break;
                                }
                            case DayOfWeek.Friday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Fri);
                                    break;
                                }
                            case DayOfWeek.Saturday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Sat);
                                    break;
                                }
                            case DayOfWeek.Sunday:
                                {
                                    revent.Days.Add((int)DayOfWeekEnum.Sun);
                                    break;
                                }
                        }
                        rectrl.PostSave(revent);
                    }
                    else
                    {
                        ectrl.PostSave(new EventCalendar.Core.Models.Event() { AllDay = e.IsAllDay, calendarId = calendar_id, Title = e.Summary, Start = e.Start.Date, End = e.End.Date, Id = 0, Categories = String.Join(",", e.Categories) });
                    }
                }
            }
        }

        private void ImportEventsToNewCalendar(IICalendarCollection collection)
        {
            var cctrl = new CalendarApiController();
            var ectrl = new EventApiController();
            var rectrl = new REventApiController();            

            //loop through calendar in collection
            foreach (var cal in collection)
            {
                //Create the new calendar
                string name = "New Imported Calendar";
                if(cal.Properties["X-WR-CALNAME"] != null) {
                    if(!String.IsNullOrEmpty(cal.Properties["X-WR-CALNAME"].Value.ToString())) {
                        name = cal.Properties["X-WR-CALNAME"].Value.ToString();
                    }
                }
                var calendar = cctrl.PostSave(new ECalendar() { Id = 0, Calendarname = name, DisplayOnSite = false, IsGCal = false, GCalFeedUrl = "", ViewMode = "month", Color = "#000000", TextColor = "#FFFFFF" });
                //loop through events in calendar
                foreach (var e in cal.Events)
                {
                    //check if it is an reccuring event
                    if (e.RecurrenceRules.Count > 0)
                    {
                        var revent = new RecurringEvent() { AllDay = e.IsAllDay, Title = e.Summary, Start = e.Start.Date, End = e.Start.Date, calendarId = calendar.Id, Categories = String.Join(",", e.Categories), Id = 0 };
                        revent.MonthlyIntervals.Add(((int)MonthlyIntervalEnum.First));
                        RecurrencePattern rp = (RecurrencePattern)e.RecurrenceRules[0];
                        switch (rp.Frequency)
                        {
                            case FrequencyType.Daily:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Daily;
                                    break;
                                }
                            case FrequencyType.Monthly:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Monthly;
                                    break;
                                }
                            case FrequencyType.Weekly:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Weekly;
                                    break;
                                }
                            default:
                                {
                                    revent.Frequency = (int)FrequencyTypeEnum.Monthly;
                                    break;
                                }
                        }
                        revent.Days.AddRange(rp.ByDay.Select(x => (int)x.DayOfWeek));
                        
                        rectrl.PostSave(revent);
                    }
                    else
                    {
                        ectrl.PostSave(new EventCalendar.Core.Models.Event() { AllDay = e.IsAllDay, calendarId = calendar.Id, Title = e.Summary, Start = e.Start.Date, End = e.End.Date, Id = 0, Categories = String.Join(",", e.Categories) });
                    }
                }
            }
        }
    }
}
