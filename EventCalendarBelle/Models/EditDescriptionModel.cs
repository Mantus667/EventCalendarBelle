using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EventCalendarBelle.Models
{
    public class EditDescriptionModel
    {
        [Required]
        public int id {get;set;}

        [Required]
        public int eventid {get;set;}

        [Required]
        public int type { get; set; }

        [Required]
        public int calendarid { get; set; }

        [Required]
        [AllowHtml]
        public string content{get;set;}

        [Required]
        public string culture { get; set; }
    }
}