using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EventCalendarBelle.Models
{
    [TableName("ec_eventdescriptions")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    [DataContract(Name = "description", Namespace = "")]
    public class EventDescription
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }

        [Column("eventid")]
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "eventId", IsRequired = true)]
        public int EventId { get; set; }

        [Column("type")]
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "type", IsRequired = true)]
        public int Type { get; set; }

        [Column("calendarid")]
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "calendarId", IsRequired = true)]
        public int CalendarId { get; set; }

        [Column("culture")]
        [StringLength(5)]
        [Required]
        [DataMember(Name = "culture", IsRequired = true)]
        public string CultureCode { get; set; }

        [Column("content")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        [AllowHtml]
        [DataMember(Name = "value")]
        public string Content { get; set; }
    }
}