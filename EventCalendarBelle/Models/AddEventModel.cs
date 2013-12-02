using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EventCalendarBelle.Models
{
    public class AddEventModel
    {
        [Required]
        [HiddenInput]
        public int calendarId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(50)]
        public string title { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        public DateTime start { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.DateTime)]
        public DateTime end { get; set; }

        [Display(Name = "Is all day?")]
        public bool allday { get; set; }

        [Required]
        [Display(Name = "Description")]
        [DataType(DataType.Html)]
        public string description { get; set; }

        [HiddenInput]
        public int selectedLocation { get; set; }

        [Display(Name = "Location")]
        public SelectList locations { get; set; }

        [HiddenInput]
        public string lat { get; set; }

        [HiddenInput]
        public string lon { get; set; }
    }
}