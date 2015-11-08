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

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "days")]
        public List<int> Days { get; set; }

        [DataMember(Name = "intervals")]
        public List<int> MonthlyIntervals { get; set; }

        [DataMember(Name = "mediaItems")]
        public List<int> MediaItems { get; set; }

        [DataMember(Name = "descriptions")]
        public List<EventDescription> Descriptions { get; set; }

        [DataMember(Name = "exceptions")]
        public List<DateException> Exceptions { get; set; }

        [DataMember(Name = "calendar")]
        public ECalendar Calendar { get; set; }

        [DataMember(Name = "location")]
        public EventLocation Location { get; set; }
    }

    public class RecurringEventListModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public bool allDay { get; set; }
        public DayOfWeekEnum day { get; set; }
        public FrequencyTypeEnum frequency { get; set; }
    }

    [DataContract(Name = "pagedREvents", Namespace = "")]
    public class PagedREventsResult
    {
        [DataMember(Name = "events")]
        public List<RecurringEvent> Events { get; set; }

        [DataMember(Name = "currentPage")]
        public long CurrentPage { get; set; }

        [DataMember(Name = "itemsPerPage")]
        public long ItemsPerPage { get; set; }

        [DataMember(Name = "totalPages")]
        public long TotalPages { get; set; }

        [DataMember(Name = "totalItems")]
        public long TotalItems { get; set; }
    }
}