using EventCalendarBelle.Models;
using ScheduleWidget.Enums;
using ScheduleWidget.ScheduledEvents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace EventCalendarBelle.Controller
{
    [PluginController("EventCalendar")]
    public class ECSurfaceController : SurfaceController
    {
        [ChildActionOnly]
        public ActionResult GetEventDetails(int id, int calendar, int type = 0)
        {
            EventLocation l = null;
            EventDetailsModel evm = null;

            var lctrl = new LocationApiController();

            if (type == 0)
            {
                //Fetch Event
                var ectrl = new EventApiController();
                var e = ectrl.GetById(id);

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                }                

                evm = new EventDetailsModel()
                {
                    Title = e.title,
                    LocationId = e.locationId,
                    Location = l,
                    Descriptions = e.descriptions
                };
                if (null != e.start)
                {
                    evm.StartDate = ((DateTime)e.start).ToString("F", CultureInfo.CurrentCulture);
                }
                if (null != e.end)
                {
                    evm.EndDate = ((DateTime)e.end).ToString("F", CultureInfo.CurrentCulture);
                }
            }
            else if (type == 1)
            {
                var ectrl = new REventApiController();
                var e = ectrl.GetById(id);
                
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                }                

                Schedule schedule = new Schedule(new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.title,
                    DaysOfWeekOptions = (DayOfWeekEnum)e.day,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.frequency,
                    MonthlyIntervalOptions = (MonthlyIntervalEnum)e.monthly_interval
                });               

                evm = new EventDetailsModel()
                {
                    Title = e.title,
                    LocationId = e.locationId,
                    Location = l,
                    //StartDate = ((DateTime)schedule.NextOccurrence(DateTime.Now)).ToString("F", CultureInfo.CurrentCulture),
                    //EndDate = ((DateTime)schedule.NextOccurrence(DateTime.Now)).ToString("F", CultureInfo.CurrentCulture),
                    Descriptions = e.descriptions
                };

                if (null != e.start)
                {
                    var start = ((DateTime)schedule.NextOccurrence(DateTime.Now));
                    evm.StartDate = new DateTime(start.Year, start.Month, start.Day, e.start.Hour, e.start.Minute, 0).ToString("F", CultureInfo.CurrentCulture);

                }
                if (null != e.end)
                {
                    var end = ((DateTime)schedule.NextOccurrence(DateTime.Now));
                    evm.EndDate = new DateTime(end.Year, end.Month, end.Day, e.end.Hour, e.end.Minute, 0).ToString("F", CultureInfo.CurrentCulture);
                }
            }

            return PartialView("EventDetails", evm);
        }
    }
}
