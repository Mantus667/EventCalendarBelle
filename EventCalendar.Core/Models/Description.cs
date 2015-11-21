using EventCalendar.Core.Enums;
using System.Runtime.Serialization;

namespace EventCalendar.Core.Models
{
    [DataContract(Name = "description", Namespace = "")]
    public class Description
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "parent")]
        public int ParentId { get; set; }

        [DataMember(Name = "type")]
        public DescriptionType DescriptionType { get; set; }

        [DataMember(Name = "culture")]
        public string CultureCode { get; set; }

        [DataMember(Name = "value")]
        public string Content { get; set; }
    }
}
