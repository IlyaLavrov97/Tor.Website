using Microsoft.AspNetCore.Http;

namespace Tor.Website.Models.Request
{
    public class ImageUploadRequest : BaseRequest
    {
        public string Id { get; set; }
        public string IsDefault { get; set; }
        public IFormFile Image { get; set; }
    }
}
