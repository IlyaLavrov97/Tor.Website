using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tor.Website.Models.DBO;

namespace Tor.Website.Models.Response
{
    [DataContract]
    public class GetArticleByIdResponse
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Preview { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public List<TorImage> Images { get; set; }
    }
}
