using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventCalendarBelle.Models
{
    public class EventDetailsModel
    {
        public string CalendarName { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }        
        public EventLocation Location { get; set; }
        public List<EventDescription> Descriptions { get; set; }

        public string GetDescription(string culture)
        {
            if (this.Descriptions.Any(x => x.CultureCode == culture))
            {
                return this.Descriptions.SingleOrDefault(x => x.CultureCode == culture).Content;
            }
            else
            {
                return "";
            }
        }
    }
}