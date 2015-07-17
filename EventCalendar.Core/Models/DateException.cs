using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.Models
{
    [DataContract(Name = "dateexception", Namespace = "")]
    public class DateException
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "date")]
        public DateTime? Date { get; set; }

        [DataMember(Name = "event")]
        public int EventId { get; set; }
    }
}
