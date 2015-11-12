using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventCalendar.Core.Models
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
        public string textColor { get; set; }
        public string categories { get; set; }
        public EventType type { get; set; }
        public int calendar { get; set; }
    }

    public class EventListModel
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public int CalendarId { get; set; }
        public int Type { get; set; }
    }
}