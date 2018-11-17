using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tor.Website.EF;
using Tor.Website.Models.DBO;
using Tor.Website.Models.Request;
using Tor.Website.Models.Response;
using TorWebApi.Controllers.Base;

namespace TorWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/articles")]
    public class ArticlesController : BaseController
    {
        public ArticlesController(DataContext context, IHostingEnvironment hostingEnvironment)
            : base (context, hostingEnvironment)
        {
        }

        [HttpPost]
        public async Task<IActionResult> GetAllArticles([FromBody] GetAllArticlesRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                var articles = await Context.Previews?.ToListAsync();

                if (articles == null)
                    return NotFound();

                return Ok(articles);
            }, request);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> GetArticleById([FromBody] GetAriticleByIdRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                GetArticleByIdResponse response = new GetArticleByIdResponse();

                var article = await Context.Articles
                    .Include(o => o.Images)
                    .Include(o => o.Preview)
                    .SingleOrDefaultAsync(m => m.ID == int.Parse(Request.Path.Value.Split('/', StringSplitOptions.None).Last()));

                if (article == null)
                    return NotFound();
                
                response.Images = article.Images.ToList();
                response.Preview = article.Preview.Preview;
                response.Title = article.Preview.Title;
                response.DateTime = article.Preview.DateTime;
                response.Content = article.Content;
                response.Id = article.ID;

                return Ok(response);
            }, request);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddArticle([FromBody] AddArticleRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                await Context.Articles.AddAsync(
                    new Article
                    {
                        Content = param.Content,
                        Preview = new ArticlePreview
                        {
                            DateTime = DateTime.UtcNow,
                            Preview = request.Preview,
                            Title = request.Title,
                        }
                    });

                await Context.SaveChangesAsync();

                return Ok();
            }, request);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveArticle([FromBody] RemoveArticleRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                Article article = await Context.Articles
                 .Include(o => o.Images)
                 .Include(o => o.Preview).FirstOrDefaultAsync(art => art.ID == param.Id);

                foreach (var image in article.Images)
                {
                    System.IO.File.Delete(HostingEnvironment.WebRootPath + image.LocalPath);
                }

                Context.Previews.Remove(article.Preview);
                Context.Images.RemoveRange(article.Images);

                await Context.SaveChangesAsync();

                Context.Articles.Remove(article);

                await Context.SaveChangesAsync();

                return Ok();
            }, request);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditArticle([FromBody] EditArticleRequest request)
        {
            return await MethodWrapper(async (param) =>
            {
                List<TorImage> images = new List<TorImage>();

                Article article = await Context.Articles
                  .Include(o => o.Preview).FirstOrDefaultAsync(art => art.ID == param.Id);

                if (!string.IsNullOrEmpty(param.Content))
                    article.Content = param.Content;

                if (!string.IsNullOrEmpty(param.Preview))
                    article.Preview.Preview = param.Preview;

                if (!string.IsNullOrEmpty(param.Title))
                    article.Preview.Title = param.Title;

                Context.Articles.Update(article);

                await Context.SaveChangesAsync();

                GetArticleByIdResponse response = new GetArticleByIdResponse
                {
                    Preview = article.Preview.Preview,
                    Title = article.Preview.Title,
                    DateTime = article.Preview.DateTime,
                    Content = article.Content,
                    Id = article.ID
                };

                return Ok(response);
            }, request);
        }
    }
}
