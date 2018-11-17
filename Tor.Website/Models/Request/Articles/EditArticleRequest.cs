using Microsoft.AspNetCore.Http;

namespace Tor.Website.Models.Request
{
    public class EditArticleRequest : BaseRequest
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Preview { get; set; }

        public string Content { get; set; }
    }
}
