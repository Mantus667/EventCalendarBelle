using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using ScheduleWidget.Enums;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace EventCalendarBelle.Models
{
    [TableName("ec_recevents")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    [DataContract(Name = "revent", Namespace = "")]
    public class RecurringEvent
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column("calendarid")]
        [DataMember(Name = "calendarid")]
        public int calendarId { get; set; }

        [Column("locationId")]
        [DataMember(Name = "locationid")]
        public int locationId { get; set; }

        [Column("title")]
        [DataMember(Name = "title")]
        public string title { get; set; }

        [Column("allday")]
        [DataMember(Name = "allday")]
        public bool allDay { get; set; }

        [Column("day")]
        [DataMember(Name = "day")]
        public int day { get; set; }

        [Column("frequency")]
        [DataMember(Name = "frequency")]
        public int frequency { get; set; }

        [Column("monthly")]
        [DataMember(Name = "monthly")]
        public int monthly_interval { get; set; }

        [Column("categories")]
        [DataMember(Name = "categories")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string categories { get; set; }

        [Column("start")]
        [DataMember(Name = "starttime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime start { get; set; }

        [Column("end")]
        [DataMember(Name = "endtime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime end { get; set; }

        [Ignore]
        [DataMember(Name = "descriptions")]
        public List<EventDescription> descriptions { get; set; }
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