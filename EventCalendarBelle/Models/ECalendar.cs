using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace EventCalendarBelle.Models
{
    [TableName("ec_calendars")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    [DataContract(Name = "calendar", Namespace = "")]
    public class ECalendar
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }

        [Column("cname")]
        [Required]
        [Display(Name = "Calendar Name")]
        [DataMember(Name = "calendarname", IsRequired = true)]
        public string Calendarname { get; set; }

        [Column("gcal")]
        [Display(Name = "Use GCal?")]
        [DataMember(Name = "isGCal")]
        public bool IsGCal { get; set; }

        [Column("visible")]
        [Display(Name = "Show on site?")]
        [DataMember(Name = "displayOnSite")]
        public bool DisplayOnSite { get; set; }

        [Column("gcalfeed")]
        [Display(Name = "GCal Feed Url")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [StringLength(255)]
        [DataMember(Name = "gcalFeedUrl")]
        public string GCalFeedUrl { get; set; }

        [Column("apikey")]
        [Display(Name = "Google Api Key")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [StringLength(255)]
        [DataMember(Name = "apikey")]
        public string GoogleAPIKey { get; set; }

        [Column("color")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Display(Name = "Event-Color")]
        [DataMember(Name = "color")]
        public string Color { get; set; }

        [Column("textcolor")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Display(Name = "Event-Text-Color")]
        [DataMember(Name = "textColor")]
        public string TextColor { get; set; }

        [Column("viewmode")]
        [Display(Name = "View Mode")]
        [DataMember(Name="viewMode")]
        public string ViewMode { get; set; }
    }
}