using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_locations")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class EventLocationDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("lname")]
        public string LocationName { get; set; }

        [Column("street")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Street { get; set; }

        [Column("zip")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ZipCode { get; set; }

        [Column("city")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string City { get; set; }

        [Column("country")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Country { get; set; }

        [Column("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        public string Longitude { get; set; }

        [Column("media")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string media { get; set; }
    }
}
