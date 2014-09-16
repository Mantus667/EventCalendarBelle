using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.Runtime.Serialization;

namespace EventCalendarBelle.Models
{
    [TableName("ec_events")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    [DataContract(Name="event", Namespace= "")]
    public class Event
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [DataMember(Name="id")]
        public int Id { get; set; }

        [Column("calendarid")]
        [DataMember(Name = "calendarid")]
        public int calendarId { get; set; }

        [Column("locationId")]
        [DataMember(Name = "locationId")]
        public int locationId { get; set; }

        [Column("title")]
        [DataMember(Name = "title")]
        public string title { get; set; }

        [Column("starttime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [DataMember(Name = "starttime")]
        public DateTime? start { get; set; }

        [Column("endtime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [DataMember(Name = "endtime")]
        public DateTime? end { get; set; }

        [Column("allday")]
        [DataMember(Name = "allday")]
        public bool allDay { get; set; }

        [Column("categories")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [DataMember(Name = "categories")]
        public string categories { get; set; }

        [Column("organisator")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [DataMember(Name = "organisator_id")]
        public int Organisator { get; set; }

        [Ignore]
        [DataMember(Name = "descriptions")]
        public List<EventDescription> descriptions { get; set; }
    }
}