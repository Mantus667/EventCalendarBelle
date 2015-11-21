using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace EventCalendar.Core.Dto
{
    [TableName("ec_descriptions")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    public class DescriptionDto
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("parent")]
        public int ParentId { get; set; }

        [Column("type")]
        public int DescriptionType { get; set; }

        [Column("culture")]
        public string CultureCode { get; set; }

        [Column("content")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Content { get; set; }
    }
}
