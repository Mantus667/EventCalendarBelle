using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_calendars")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class CalendarDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("cname")]
        public string Calendarname { get; set; }

        [Column("gcal")]
        public bool IsGCal { get; set; }

        [Column("visible")]
        public bool DisplayOnSite { get; set; }

        [Column("gcalfeed")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string GCalFeedUrl { get; set; }

        [Column("apikey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string GoogleAPIKey { get; set; }

        [Column("color")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Color { get; set; }

        [Column("textcolor")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string TextColor { get; set; }

        [Column("viewmode")]
        public string ViewMode { get; set; }
    }
}
