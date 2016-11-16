using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Access;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
    [Route("api/page")]
    public class ApiPageController : RmApiControllerBase
    {
        private class PageBrief
        {
            public int PageId { get; set; }

            public string Path { get; set; }
            public string Title { get; set; }

            public int HomeOrder { get; set; }
            public int SplashOrder { get; set; }
            public int NavbarOrder { get; set; }

            public string ThumbnailImage { get; set; }
            public string SplashImage { get; set; }
        }

        public ApiPageController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        [HttpGet("navbar")]
        public IActionResult NavbarPages()
        {
            // TODO: cache this

            var query = from p in _context.Pages
                where p.NavbarOrder > 0
                select new PageBrief
                {
                    PageId = p.PageId,
                    Path = p.Path,
                    Title = p.Title,

                    HomeOrder = p.HomeOrder,
                    SplashOrder = p.SplashOrder,
                    NavbarOrder = p.NavbarOrder,

                    ThumbnailImage = p.ThumbnailImage,
                    SplashImage = p.SplashImage
                };
            return Json(query.ToList());
        }

        [HttpGet("home")]
        public IActionResult HomePages()
        {
            // TODO: cache this

            var query = from p in _context.Pages
                        where p.HomeOrder > 0 || p.SplashOrder > 0
                        select new PageBrief
                        {
                            PageId = p.PageId,
                            Path = p.Path,
                            Title = p.Title,

                            HomeOrder = p.HomeOrder,
                            SplashOrder = p.SplashOrder,
                            NavbarOrder = p.NavbarOrder,

                            ThumbnailImage = p.ThumbnailImage,
                            SplashImage = p.SplashImage
                        };
            return Json(query.ToList());
        }

        [HttpGet("page")]
        public IActionResult GetPage(string path)
        {
            var page = _context.Pages.FirstOrDefault(p => p.Path == path);
            if (page == null || !page.Enabled)
            {
                return Json("page not found");
            }

            if (page.RequireLogin || page.RequireFullMember || page.RequireAdmin)
            {
                var user = UserManager.GetUser(HttpContext.Session);
                if (user == null ||
                    (page.RequireFullMember && !UserManager.IsFullMember(user)) ||
                    (page.RequireAdmin && !UserManager.IsAdmin(HttpContext.Session)))
                {
                    return Json("not authorized");
                }
            }

            return Json(page);
        }
    }
}
