using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Models
{
    [TableName("ec_usettings")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    [DataContract(Name = "user", Namespace = "")]
    public class UserSettings
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column("userid")]
        [DataMember(Name = "user_id")]
        public int UserId { get; set; }

        [Column("createcalendar")]
        [DataMember(Name = "canCreateCalendar")]
        public bool CanCreateCalendar { get; set; }

        [Column("deletecalendar")]
        [DataMember(Name = "canDeleteCalendar")]
        public bool CanDeleteCalendar { get; set; }

        [Column("createlocations")]
        [DataMember(Name = "canCreateLocations")]
        public bool CanCreateLocations { get; set; }

        [Column("deletelocations")]
        [DataMember(Name = "canDeleteLocations")]
        public bool CanDeleteLocations { get; set; }

        [Column("createevents")]
        [DataMember(Name = "canCreateEvents")]
        public bool CanCreateEvents { get; set; }

        [Column("deleteevents")]
        [DataMember(Name = "canDeleteEvents")]
        public bool CanDeleteEvents { get; set; }

        [Column("calendar")]
        [DataMember(Name = "calendar")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Calendar { get; set; }

        [Ignore]
        [DataMember(Name = "calendar_array")]
        public IEnumerable<string> AllowedCalendar { 
            get {
                if (!String.IsNullOrEmpty(this.Calendar))
                {
                    return this.Calendar.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    return Enumerable.Empty<string>();
                }
            } 
        }

        [Column("locations")]
        [DataMember(Name = "locations")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Locations { get; set; }

        [Ignore]
        [DataMember(Name = "locations_array")]
        public IEnumerable<string> AllowedLocations
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Locations))
                {
                    return this.Locations.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    return Enumerable.Empty<string>();
                }
            }
        }

        [Ignore]
        [DataMember(Name = "userName")]
        public string UserName { get; set; }
    }

    [DataContract(Name = "pagedUsers", Namespace = "")]
    public class PagedUserResult
    {
        [DataMember(Name = "user")]
        public List<UserSettings> User { get; set; }

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
