using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.Models
{
    public class GridEditorListValueModel
    {
        public int CalendarId { get; set; }
        public int Quantity { get; set; }
        public bool Teaser { get; set; }
        public string Title { get; set; }
        public bool Forward { get; set; }
        public IEnumerable<EventsOverviewModel> Events { get; set; }
    }
}
