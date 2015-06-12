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
                //Fetch Event
                var e = EventService.GetEvent(id);

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = LocationService.GetLocation(e.locationId);
                }
                
                evm = new EventDetailsModel()
                {
                    CalendarId = calendar,
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
                if (e.Organiser != null && e.Organiser != 0)
                {
                    var member = ms.GetById(e.Organiser);
                    evm.Organisor = new Organisor() { Name = member.Name, Email = member.Email };
                }
            }
            else if (type == 1)
            {
                var e = RecurringEventService.GetRecurringEvent(id);
                
                if (e.locationId != 0)
                {
                    l = LocationService.GetLocation(e.locationId);
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
                    CalendarId = calendar,
                    Title = e.title,
                    LocationId = e.locationId,
                    Location = l,
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
                if (e.Organiser != null && e.Organiser != 0)
                {
                    var member = ms.GetById(e.Organiser);
                    evm.Organisor = new Organisor() { Name = member.Name, Email = member.Email };
                }
            }

            return PartialView("EventDetails", evm);
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

                var start = (DateTime)e.start;
                evt.Start = new iCalDateTime(start.ToUniversalTime());
                if(e.end != null) {
                    var end = (DateTime)e.end;
                    evt.End = new iCalDateTime(end.ToUniversalTime());
                }
                //evt.Description = this.GetDescription(e, CultureInfo.CurrentCulture.ToString());
                evt.Summary = e.title;
                evt.IsAllDay = e.allDay;

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                    //evt.GeographicLocation = new GeographicLocation(Convert.ToDouble(l.lat,CultureInfo.InvariantCulture), Convert.ToDouble(l.lon, CultureInfo.InvariantCulture));
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

                //evt.Description = this.GetDescription(e, CultureInfo.CurrentCulture.ToString());
                evt.Summary = e.title;
                evt.IsAllDay = e.allDay;

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                    //evt.GeographicLocation = new GeographicLocation(Convert.ToDouble(l.lat, CultureInfo.InvariantCulture), Convert.ToDouble(l.lon, CultureInfo.InvariantCulture));
                }

                RecurrencePattern rp = null;
                var frequency = (FrequencyTypeEnum)e.frequency;
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
                switch (e.day)
                {
                    case (int)DayOfWeekEnum.Mon:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Monday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Tue:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Tuesday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Wed:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Wednesday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Thu:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Thursday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Fri:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Friday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Sat:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Saturday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Sun:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Sunday));
                            break;
                        }
                }
                evt.RecurrenceRules.Add(rp);

                Schedule schedule = new Schedule(new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.title,
                    DaysOfWeekOptions = (DayOfWeekEnum)e.day,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.frequency,
                    MonthlyIntervalOptions = (MonthlyIntervalEnum)e.monthly_interval
                });

                var occurence = Convert.ToDateTime(schedule.NextOccurrence(DateTime.Now));
                evt.Start = new iCalDateTime(new DateTime(occurence.Year, occurence.Month, occurence.Day, e.start.Hour, e.start.Minute, 0));
                //evt.End = new iCalDateTime(new DateTime(occurence.Year, occurence.Month, occurence.Day, e.end.Hour, e.end.Minute, 0));

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

                var start = (DateTime)e.start;
                evt.Start = new iCalDateTime(start.ToUniversalTime());
                if (e.end != null)
                {
                    var end = (DateTime)e.end;
                    evt.End = new iCalDateTime(end.ToUniversalTime());
                }
                
                evt.Description = this.GetDescription(e, CultureInfo.CurrentCulture.ToString());
                evt.Summary = e.title;
                evt.IsAllDay = e.allDay;
                evt.Categories.AddRange(e.categories.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                    //evt.GeographicLocation = new GeographicLocation(Convert.ToDouble(l.lat,CultureInfo.InvariantCulture), Convert.ToDouble(l.lon, CultureInfo.InvariantCulture));
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
                evt.Summary = e.title;
                evt.IsAllDay = e.allDay;
                evt.Categories.AddRange(e.categories.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList());

                //If it has a location fetch it
                if (e.locationId != 0)
                {
                    l = lctrl.GetById(e.locationId);
                    evt.Location = l.LocationName + "," + l.Street + "," + l.ZipCode + " " + l.City + "," + l.Country;
                    //evt.GeographicLocation = new GeographicLocation(Convert.ToDouble(l.lat, CultureInfo.InvariantCulture), Convert.ToDouble(l.lon, CultureInfo.InvariantCulture));
                }

                RecurrencePattern rp = null;
                var frequency = (FrequencyTypeEnum)e.frequency;
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
                switch (e.day)
                {
                    case (int)DayOfWeekEnum.Mon:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Monday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Tue:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Tuesday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Wed:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Wednesday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Thu:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Thursday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Fri:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Friday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Sat:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Saturday));
                            break;
                        }
                    case (int)DayOfWeekEnum.Sun:
                        {
                            rp.ByDay.Add(new WeekDay(DayOfWeek.Sunday));
                            break;
                        }
                }
                evt.RecurrenceRules.Add(rp);

                Schedule schedule = new Schedule(new ScheduleWidget.ScheduledEvents.Event()
                {
                    Title = e.title,
                    DaysOfWeekOptions = (DayOfWeekEnum)e.day,
                    FrequencyTypeOptions = (FrequencyTypeEnum)e.frequency,
                    MonthlyIntervalOptions = (MonthlyIntervalEnum)e.monthly_interval
                });

                var occurence = Convert.ToDateTime(schedule.NextOccurrence(DateTime.Now));
                evt.Start = new iCalDateTime(new DateTime(occurence.Year, occurence.Month, occurence.Day, e.start.Hour, e.start.Minute, 0));

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
            if (e.descriptions != null && e.descriptions.Any(x => x.CultureCode == culture))
            {
                return Umbraco.StripHtml(e.descriptions.SingleOrDefault(x => x.CultureCode == culture).Content).ToString();
            }
            else
            {
                return "";
            }
        }
        private string GetDescription(EventCalendar.Core.Models.RecurringEvent e, string culture)
        {
            if (e.descriptions != null && e.descriptions.Any(x => x.CultureCode == culture))
            {
                return e.descriptions.SingleOrDefault(x => x.CultureCode == culture).Content;
            }
            else
            {
                return "";
            }
        }
    }
}
