using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_events")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class EventDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("calendarid")]
        public int calendarId { get; set; }

        [Column("locationId")]
        public int locationId { get; set; }

        [Column("title")]
        public string title { get; set; }

        [Column("starttime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? start { get; set; }

        [Column("endtime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? end { get; set; }

        [Column("allday")]
        public bool allDay { get; set; }

        [Column("categories")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string categories { get; set; }

        [Column("organiser")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int Organiser { get; set; }

        [Column("media")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string media { get; set; }
    }
}
