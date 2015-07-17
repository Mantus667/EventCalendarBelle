using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Runtime.Serialization;

namespace EventCalendar.Core.Models
{
    [DataContract(Name = "calendar", Namespace = "")]
    public class ECalendar
    {
        [HiddenInput(DisplayValue = false)]
        [Required]
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Calendar Name")]
        [DataMember(Name = "calendarname", IsRequired = true)]
        public string Calendarname { get; set; }

        [Display(Name = "Use GCal?")]
        [DataMember(Name = "isGCal")]
        public bool IsGCal { get; set; }

        [Display(Name = "Show on site?")]
        [DataMember(Name = "displayOnSite")]
        public bool DisplayOnSite { get; set; }

        [Display(Name = "GCal Feed Url")]
        [StringLength(255)]
        [DataMember(Name = "gcalFeedUrl")]
        public string GCalFeedUrl { get; set; }

        [Display(Name = "Google Api Key")]
        [StringLength(255)]
        [DataMember(Name = "apikey")]
        public string GoogleAPIKey { get; set; }

        [Display(Name = "Event-Color")]
        [DataMember(Name = "color")]
        public string Color { get; set; }

        [Display(Name = "Event-Text-Color")]
        [DataMember(Name = "textColor")]
        public string TextColor { get; set; }

        [Display(Name = "View Mode")]
        [DataMember(Name="viewMode")]
        public string ViewMode { get; set; }
    }
}