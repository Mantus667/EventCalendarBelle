using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventCalendarBelle.Models
{
    public enum EventType {
        Normal = 0, Recurring = 1
    }

    public class EventsOverviewModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public bool allDay { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        public EventType type { get; set; }
        public int calendar { get; set; }
    }
}