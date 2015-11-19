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
    
    [DataContract(Name = "location", Namespace = "")]
    public class EventLocation
    {
        [DataMember(Name = "id", IsRequired = true)]
        public int Id { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string LocationName { get; set; }

        [DataMember(Name = "street")]
        public string Street { get; set; }

        [DataMember(Name = "zipCode")]
        public string ZipCode { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "lat", IsRequired = true)]
        public string Latitude { get; set; }

        [DataMember(Name = "lon", IsRequired = true)]
        public string Longitude { get; set; }

        [DataMember(Name = "mediaItems")]
        public List<int> MediaItems { get; set; }
    }

    [DataContract(Name = "pagedLocations", Namespace = "")]
    public class PagedLocationsResult
    {
        [DataMember(Name = "locations")]
        public List<EventLocation> Locations { get; set; }

        [DataMember(Name = "currentPage")]
        public long CurrentPage { get; set; }

        [DataMember(Name = "itemsPerPage")]
        public long ItemsPerPage { get; set; }

        [DataMember(Name = "totalPages")]
        public long TotalPages { get; set; }

        [DataMember(Name = "totalItems")]
        public long TotalItems { get; set; }
    }
}