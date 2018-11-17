using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tor.Website.EF;
using Tor.Website.Models.Request;

namespace TorWebApi.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly DataContext Context;
        protected readonly IHostingEnvironment HostingEnvironment;

        public BaseController(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            Context = context;
            HostingEnvironment = hostingEnvironment;
        }

        protected async Task<IActionResult> MethodWrapper<TParam>(Func<TParam, Task<IActionResult>> func, TParam param)
            where TParam : BaseRequest
        {
            try
            {
                return await func(param);
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
    }
}
