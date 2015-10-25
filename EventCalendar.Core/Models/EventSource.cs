using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.Models
{
    public class EventSource
    {
        public string url { get; set; }
        public readonly string type = "POST";
        public string headers { get; set; }
        public object data { get; set; }
    }
}
