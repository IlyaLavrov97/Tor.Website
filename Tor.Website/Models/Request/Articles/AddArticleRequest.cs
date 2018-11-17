using System;

namespace Tor.Website.Models.Request
{
    public class AddArticleRequest : BaseRequest
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string Preview { get; set; }

        public DateTime DateTime { get; set; }
    }
}
