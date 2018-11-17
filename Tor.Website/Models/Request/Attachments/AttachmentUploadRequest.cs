using Microsoft.AspNetCore.Http;

namespace Tor.Website.Models.Request
{
    public class AttachmentUploadRequest : BaseRequest
    {
        public IFormFile Attacment { get; set; }
    }
}
