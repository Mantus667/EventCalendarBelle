using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_recevents")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class RecurringEventDto
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

        [Column("allday")]
        public bool allDay { get; set; }

        [Column("day")]
        public string day { get; set; }

        [Column("frequency")]
        public int frequency { get; set; }

        [Column("monthly")]
        public string monthly_interval { get; set; }

        [Column("categories")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string categories { get; set; }

        [Column("start")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime start { get; set; }

        [Column("end")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime end { get; set; }

        [Column("organiser")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int Organiser { get; set; }

        [Column("rangeStart")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int range_start { get; set; }

        [Column("rangeEnd")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int range_end { get; set; }
    }
}
