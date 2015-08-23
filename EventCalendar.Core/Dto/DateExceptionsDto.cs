using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_dateexceptions")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class DateExceptionDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("event_id")]
        public int EventId { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }
    }
}
