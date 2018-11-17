using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Tor.Website.Models.DBO
{
    [DataContract]
    public class TorImage
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [Required]
        public string Name { get; set; }

        [DataMember]
        [Required]
        public bool IsDefault { get; set; }
        
        [DataMember]
        public string ExternalPath { get; set; }

        [IgnoreDataMember]
        [Required]
        public string LocalPath { get; set; }

        [IgnoreDataMember]
        public int ArticleId { get; set; }
        
        [IgnoreDataMember]
        [Required]
        public Article Article { get; set; }
    }
}
