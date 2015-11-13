using EventCalendar.Core.Models;
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
using DDay.iCal;
using DDay.iCal.Serialization;
using EventCalendar.Core.Services;
using Umbraco.Web;

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
            var ms = Services.MemberService;

            if (type == 0)
            {
                evm =  EventService.GetEventDetails(id);
            }
            else if (type == 1)
            {
                evm = RecurringEventService.GetEventDetails(id);
            }

            return PartialView("EventDetails", evm);
        }

        [ChildActionOnly]
        public ActionResult GetLocationDetails(int id)
        {
            var location = LocationService.GetLocation(id);
            return PartialView("LocationDetails", location);
        }

        public ActionResult GetIcsForEvent(int id, int type = 0)
        {
            DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();

            var lctrl = new LocationApiController();
            EventLocation l = null;

            // Create the event, and add it to the iCalendar
            DDay.iCal.Event evt = iCal.Create<DDay.iCal.Event>();

            if (type == 0)
            {
                //Fetch Event
                var ectrl = new EventApiController();
                var e = ectrl.GetById(id);

                var start = (DateTime)e.Start;
                evt.Start = new iCalDateTime(start.ToUniversalTime());
                if(e.End != null) {
                    var end = (DateTime)e.End;
                    evt.End = new iCalDateTime(end.ToUniversalTime());
                }
                evt.Summary = e.Title;
                evt.IsAllDay = e.AllDay;

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                }

                var attendes = new List<IAttendee>();

                if (e.Organiser != null && e.Organiser != 0)
                {
                    var ms = Services.MemberService;
                    var member = ms.GetById(e.Organiser);
                    string attendee = "MAILTO:" + member.Email;
                    IAttendee reqattendee = new DDay.iCal.Attendee(attendee)
                    {
                        CommonName = member.Name,
                        Role = "REQ-PARTICIPANT"
                    };
                    attendes.Add(reqattendee);
                }

                if (attendes != null && attendes.Count > 0)
                {
                    evt.Attendees = attendes;
                }
            }
            else if (type == 1)
            {
                var ectrl = new REventApiController();
                var e = ectrl.GetById(id);

                evt.Summary = e.Title;
                evt.IsAllDay = e.AllDay;

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                }

                RecurrencePattern rp = null;
                rp = new RecurrencePattern();
                var frequency = (FrequencyTypeEnum)e.Frequency;
                switch (frequency)
                {
                    case FrequencyTypeEnum.Daily:
                        {
                            rp = new RecurrencePattern(FrequencyType.Daily);
                            break;
                        }
                    case FrequencyTypeEnum.Monthly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Monthly);
                            break;
                        }
                    case FrequencyTypeEnum.Weekly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Weekly);
                            break;
                        }
                    case FrequencyTypeEnum.Yearly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Yearly);
                            break;
                        }
                    default:
                        {
                            rp = new RecurrencePattern(FrequencyType.Monthly);
                            break;
                        }
                }
                rp.ByDay.AddRange(e.Days.Select(x => new WeekDay(x.ToString())));
                
                evt.RecurrenceRules.Add(rp);

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.Title,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.Frequency
                };

                foreach (var day in e.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in e.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                Schedule schedule = new Schedule(tmp_event);

                var occurence = Convert.ToDateTime(schedule.NextOccurrence(DateTime.Now));
                evt.Start = new iCalDateTime(new DateTime(occurence.Year, occurence.Month, occurence.Day, e.Start.Hour, e.Start.Minute, 0));

                var attendes = new List<IAttendee>();

                if (e.Organiser != null && e.Organiser != 0)
                {
                    var ms = Services.MemberService;
                    var member = ms.GetById(e.Organiser);
                    string attendee = "MAILTO:" + member.Email;
                    IAttendee reqattendee = new DDay.iCal.Attendee(attendee)
                    {
                        CommonName = member.Name,
                        Role = "REQ-PARTICIPANT"
                    };
                    attendes.Add(reqattendee);
                }

                if (attendes != null && attendes.Count > 0)
                {
                    evt.Attendees = attendes;
                }
            }

            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.
            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            // Get a serializer for our object
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, evt.Summary + ".ics");
        }

        public ActionResult GetIcsForCalendar(int id)
        {
            DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();
            

            var lctrl = new LocationApiController();
            var calctrl = new CalendarApiController();
            var cal = calctrl.GetById(id);

            EventLocation l = null;

            iCal.Properties.Add(new CalendarProperty("X-WR-CALNAME", cal.Calendarname));

            //Get normal events for calendar
            var nectrl = new EventApiController();
            foreach (var e in nectrl.GetAll().Where(x => x.calendarId == id))
            {
                // Create the event, and add it to the iCalendar
                DDay.iCal.Event evt = iCal.Create<DDay.iCal.Event>();

                var start = (DateTime)e.Start;
                evt.Start = new iCalDateTime(start.ToUniversalTime());
                if (e.End != null)
                {
                    var end = (DateTime)e.End;
                    evt.End = new iCalDateTime(end.ToUniversalTime());
                }
                
                evt.Description = this.GetDescription(e, CultureInfo.CurrentCulture.ToString());
                evt.Summary = e.Title;
                evt.IsAllDay = e.AllDay;
                evt.Categories.AddRange(e.Categories.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                }

                var attendes = new List<IAttendee>();

                if (e.Organiser != null && e.Organiser != 0)
                {
                    var ms = Services.MemberService;
                    var member = ms.GetById(e.Organiser);
                    string attendee = "MAILTO:" + member.Email;
                    IAttendee reqattendee = new DDay.iCal.Attendee(attendee)
                    {
                        CommonName = member.Name,
                        Role = "REQ-PARTICIPANT"
                    };
                    attendes.Add(reqattendee);
                }

                if (attendes != null && attendes.Count > 0)
                {
                    evt.Attendees = attendes;
                }
            }

            //Get recurring events
            var rectrl = new REventApiController();
            foreach(var e in rectrl.GetAll().Where(x => x.calendarId == id)) {
                // Create the event, and add it to the iCalendar
                DDay.iCal.Event evt = iCal.Create<DDay.iCal.Event>();

                evt.Description = this.GetDescription(e, CultureInfo.CurrentCulture.ToString());
                evt.Summary = e.Title;
                evt.IsAllDay = e.AllDay;
                evt.Categories.AddRange(e.Categories.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                }

                RecurrencePattern rp = null;
                var frequency = (FrequencyTypeEnum)e.Frequency;
                switch (frequency)
                {
                    case FrequencyTypeEnum.Daily:
                        {
                            rp = new RecurrencePattern(FrequencyType.Daily);
                            break;
                        }
                    case FrequencyTypeEnum.Monthly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Monthly);
                            break;
                        }
                    case FrequencyTypeEnum.Weekly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Weekly);
                            break;
                        }
                    case FrequencyTypeEnum.Yearly:
                        {
                            rp = new RecurrencePattern(FrequencyType.Yearly);
                            break;
                        }
                    default:
                        {
                            rp = new RecurrencePattern(FrequencyType.Monthly);
                            break;
                        }
                }
                rp.ByDay.AddRange(e.Days.Select(x => new WeekDay(x.ToString())));
                
                evt.RecurrenceRules.Add(rp);

                var tmp_event = new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.Title,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.Frequency
                };

                foreach (var day in e.Days)
                {
                    tmp_event.DaysOfWeekOptions = tmp_event.DaysOfWeekOptions | (DayOfWeekEnum)day;
                }
                foreach (var i in e.MonthlyIntervals)
                {
                    tmp_event.MonthlyIntervalOptions = tmp_event.MonthlyIntervalOptions | (MonthlyIntervalEnum)i;
                }

                Schedule schedule = new Schedule(tmp_event);

                var occurence = Convert.ToDateTime(schedule.NextOccurrence(DateTime.Now));
                evt.Start = new iCalDateTime(new DateTime(occurence.Year, occurence.Month, occurence.Day, e.Start.Hour, e.Start.Minute, 0));

                var attendes = new List<IAttendee>();

                if (e.Organiser != null && e.Organiser != 0)
                {
                    var ms = Services.MemberService;
                    var member = ms.GetById(e.Organiser);
                    string attendee = "MAILTO:" + member.Email;
                    IAttendee reqattendee = new DDay.iCal.Attendee(attendee)
                    {
                        CommonName = member.Name,
                        Role = "REQ-PARTICIPANT"
                    };
                    attendes.Add(reqattendee);
                }

                if (attendes != null && attendes.Count > 0)
                {
                    evt.Attendees = attendes;
                }
            }

            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.
            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            // Get a serializer for our object
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, cal.Calendarname + ".ics");
        }

        private string GetDescription(EventCalendar.Core.Models.Event e, string culture)
        {
            if (e.Descriptions != null && e.Descriptions.Any(x => x.CultureCode == culture))
            {
                return Umbraco.StripHtml(e.Descriptions.SingleOrDefault(x => x.CultureCode == culture).Content).ToString();
            }
            else
            {
                return "";
            }
        }
        private string GetDescription(EventCalendar.Core.Models.RecurringEvent e, string culture)
        {
            if (e.Descriptions != null && e.Descriptions.Any(x => x.CultureCode == culture))
            {
                return e.Descriptions.SingleOrDefault(x => x.CultureCode == culture).Content;
            }
            else
            {
                return "";
            }
        }
    }
}
