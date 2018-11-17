using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Tor.Website.Models.DBO
{
    public class ArticlePreview
    {
        [Key]
        public int ArticleID { get; set; }

        [DataMember]
        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [DataMember]
        [Required]
        public string Preview { get; set; }

        [DataMember]
        public string DefaultImagePath { get; set; }

        [DataMember]
        [Required]
        public DateTime DateTime { get; set; }

        [IgnoreDataMember]
        [Required]
        public Article Article { get; set; }
    }
}
