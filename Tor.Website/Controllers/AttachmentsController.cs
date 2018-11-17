using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tor.Website.EF;
using Tor.Website.Models.DBO;
using Tor.Website.Models.Request;
using TorWebApi.Controllers.Base;

namespace TorWebApi.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json", "multipart/form-data")]
    [Route("api/attachments")]
    public class AttachmentsController : BaseController
    {
        public AttachmentsController(DataContext context, IHostingEnvironment hostingEnvironment) : base(context, hostingEnvironment)
        {
        }

        [HttpPost]
        public async Task<IActionResult> GetAllAttachments([FromBody] GetAllAttachmentsRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                var articles = await Context.Attachments?.ToListAsync();

                if (articles == null)
                    return NotFound();

                return Ok(articles);
            }, request);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveAttachmentRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                Attachment attachment = await Context.Attachments.FirstOrDefaultAsync(pre => pre.ID == param.Id);

                if (attachment == null)
                {
                    ModelState.AddModelError("error", "Файл не был загружен или уже удален.");
                    return BadRequest(ModelState);
                }

                System.IO.File.Delete(HostingEnvironment.WebRootPath + attachment.LocalPath);

                Context.Remove(attachment);

                await Context.SaveChangesAsync();

                return Ok();
            }, request);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachmentImage(AttachmentUploadRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                string fileName = $"{param.Attacment.FileName}";

                // путь к папке Files
                string path = $"/Attachments/{fileName}";

                if (System.IO.File.Exists(HostingEnvironment.WebRootPath + path))
                {
                    ModelState.AddModelError("error", "Файл уже был загружен.");
                    return BadRequest(ModelState);
                }

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(HostingEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await param.Attacment.CopyToAsync(fileStream);
                }

                Attachment file = new Attachment { Name = param.Attacment.FileName, LocalPath = path, ContentSize = param.Attacment.Length };

                Context.Attachments.Add(file);

                await Context.SaveChangesAsync();

                file.ExternalPath = $"{Request.Scheme}://{Request.Host.Value}/api/attachments/{file.ID}";

                Context.Attachments.Update(file);

                await Context.SaveChangesAsync();

                return Ok();
            }, request);
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".pdf", "application/pdf"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".ppt", "application/vnd.ms-powerpoint"},
            };
        }
    }
}
