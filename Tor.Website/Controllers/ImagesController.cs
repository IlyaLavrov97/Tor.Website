using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [Route("api/images")]
    public class ImagesController : BaseController
    {
        public ImagesController(DataContext context, IHostingEnvironment hostingEnvironment)
            :base(context, hostingEnvironment)
        {
        }

        // GET: api/images/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            try
            {
                TorImage image = await Context.Images.FirstOrDefaultAsync(pre => pre.Id == id);

                if (image == null)
                {
                    ModelState.AddModelError("error", "Картинка отсутствует на сервере.");
                    return BadRequest(ModelState);
                }

                var memory = new MemoryStream();
                
                using (var fileStream = new FileStream(HostingEnvironment.WebRootPath + image.LocalPath, FileMode.Open, FileAccess.Read))
                {
                    await fileStream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, GetContentType(image.LocalPath), Path.GetFileName(image.LocalPath));
            }
            catch (Exception ex)
            {
                ModelState.Clear();
                ModelState.AddModelError("error", "Ошибка сервера :(");
#if DEBUG
                ModelState.AddModelError("ex", string.IsNullOrEmpty(ex.StackTrace) ? ex.InnerException?.StackTrace ?? string.Empty : ex.StackTrace);
#endif
                return BadRequest(ModelState);
            }
            
        }
        
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveImageRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                TorImage image = await Context.Images.FirstOrDefaultAsync(pre => pre.Id == param.Id);

                if (image == null)
                {
                    ModelState.AddModelError("error", "Картинка не была загружена или уже удалена.");
                    return BadRequest(ModelState);
                }

                System.IO.File.Delete(HostingEnvironment.WebRootPath + image.LocalPath);

                Context.Remove(image);

                await Context.SaveChangesAsync();

                return Ok();
            }, request);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAtricleImage(ImageUploadRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                if (param == null)
                    return BadRequest();

                if (!int.TryParse(param.Id, out int articleId) || !bool.TryParse(param.IsDefault, out bool isDefaultImage))
                    return BadRequest();

                string fileName = $"{param.Id}_{param.Image.FileName}";

                // путь к папке Files
                string path = $"/Images/{fileName}";

                if (System.IO.File.Exists(HostingEnvironment.WebRootPath + path))
                {
                    ModelState.AddModelError("error", "Картинка уже была загружена.");
                    return BadRequest(ModelState);
                }

                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(HostingEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await param.Image.CopyToAsync(fileStream);
                }

                TorImage file = new TorImage { ArticleId = articleId, Name = param.Image.FileName, LocalPath = path, IsDefault = isDefaultImage };

                Context.Images.Add(file);

                await Context.SaveChangesAsync();

                file.ExternalPath = $"{Request.Scheme}://{Request.Host.Value}/api/images/{file.Id}";

                Context.Images.Update(file);

                await Context.SaveChangesAsync();

                if (isDefaultImage)
                {
                    ArticlePreview articlePreview = await Context.Previews.FirstOrDefaultAsync(pre => pre.ArticleID == articleId);

                    // if exist old default image- remove it
                    if (!string.IsNullOrEmpty(articlePreview.DefaultImagePath))
                    {
                        string oldDefaultStr = articlePreview.DefaultImagePath.Split('/').LastOrDefault();

                        TorImage image = await Context.Images.FirstOrDefaultAsync(pre => pre.Id == int.Parse(oldDefaultStr));

                        System.IO.File.Delete(HostingEnvironment.WebRootPath + image.LocalPath);

                        Context.Images.Remove(image);

                        await Context.SaveChangesAsync();
                    }

                    articlePreview.DefaultImagePath = file.ExternalPath;

                    Context.Update(articlePreview);

                    await Context.SaveChangesAsync();
                }

                return Ok();
            }, request);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"}
            };
        }
    }
}
