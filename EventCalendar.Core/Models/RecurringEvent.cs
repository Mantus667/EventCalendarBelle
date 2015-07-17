using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ScheduleWidget.Enums;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EventCalendar.Core.Models
{
    
    [DataContract(Name = "revent", Namespace = "")]
    public class RecurringEvent
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "calendarid")]
        public int calendarId { get; set; }

        [DataMember(Name = "locationid")]
        public int locationId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "allday")]
        public bool AllDay { get; set; }

        [DataMember(Name = "frequency")]
        public int Frequency { get; set; }

        [DataMember(Name = "categories")]
        public string Categories { get; set; }

        [DataMember(Name = "starttime")]
        public DateTime Start { get; set; }

        [DataMember(Name = "endtime")]
        public DateTime End { get; set; }

        [DataMember(Name = "organiser_id")]
        public int Organiser { get; set; }

        [DataMember(Name = "range_start")]
        public int range_start { get; set; }

        [DataMember(Name = "range_end")]
        public int range_end { get; set; }

        [DataMember(Name = "days")]
        public List<int> Days { get; set; }

        [DataMember(Name = "intervals")]
        public List<int> MonthlyIntervals { get; set; }

        [DataMember(Name = "descriptions")]
        public List<EventDescription> Descriptions { get; set; }

        [DataMember(Name = "exceptions")]
        public List<DateException> Exceptions { get; set; }
    }

    public class RecurringEventListModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public DayOfWeekEnum day { get; set; }
        public FrequencyTypeEnum frequency { get; set; }
    }

    public class EditRecurringEventModel
    {
        [HiddenInput]
        public int id { get; set; }

        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Is all day?")]
        public bool allDay { get; set; }

        [Display(Name = "Day of the event")]
        public DayOfWeekEnum day { get; set; }

        [Display(Name = "Time period")]
        public FrequencyTypeEnum frequency { get; set; }

        [Display(Name = "Monthly period")]
        public MonthlyIntervalEnum monthly { get; set; }

        [HiddenInput]
        public int selectedLocation { get; set; }

        [Display(Name = "Location")]
        public SelectList locations { get; set; }

        [HiddenInput]
        public int calendar { get; set; }

        public Dictionary<string, EventDescription> Descriptions { get; set; }
    }
}