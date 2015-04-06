using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendarBelle.Models
{
    public class EventSource
    {
        public string url { get; set; }
        public EventSourceData data { get; set; }
        public readonly string type = "GET";
    }

    public class EventSourceData
    {
        public int id { get; set; }
    }
}
