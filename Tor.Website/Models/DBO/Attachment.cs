using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tor.Website.Models.DBO
{
    [DataContract]
    public class Attachment
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        [Required]
        public string Name { get; set; }

        [DataMember]
        public string ExternalPath { get; set; }

        [IgnoreDataMember]
        [Required]
        public string LocalPath { get; set; }

        [DataMember]
        [Required]
        public long ContentSize { get; set; }
    }
}
