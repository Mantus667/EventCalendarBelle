using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.Runtime.Serialization;

namespace EventCalendar.Core.Models
{
    [DataContract(Name="event", Namespace= "")]
    public class Event
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "calendarid")]
        public int calendarId { get; set; }

        [DataMember(Name = "locationId")]
        public int locationId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "starttime")]
        public DateTime? Start { get; set; }

        [DataMember(Name = "endtime")]
        public DateTime? End { get; set; }

        [DataMember(Name = "allday")]
        public bool AllDay { get; set; }

        [DataMember(Name = "categories")]
        public string Categories { get; set; }

        [DataMember(Name = "organiser_id")]
        public int Organiser { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "mediaItems")]
        public List<int> MediaItems { get; set; }

        [DataMember(Name = "descriptions")]
        public List<EventDescription> Descriptions { get; set; }
    }

    [DataContract(Name = "pagedEvents", Namespace = "")]
    public class PagedEventsResult
    {
        [DataMember(Name = "events")]
        public List<Event> Events { get; set; }

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