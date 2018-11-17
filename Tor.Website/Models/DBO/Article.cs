using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tor.Website.Models.DBO
{
    [DataContract]
    public class Article
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        [Required]
        public string Content { get; set; }

        [IgnoreDataMember]
        public ArticlePreview Preview { get; set; }

        [IgnoreDataMember]
        public ICollection<TorImage> Images { get; set; }
    }
}
