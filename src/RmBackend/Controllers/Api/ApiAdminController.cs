using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Access;
using RmBackend.Data;
using RmBackend.Framework;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
    [Route("api/admin")]
    [RequireLogin(RequireAdmin = true)]
    public class ApiAdminController : RmApiControllerBase
    {
        private RmLoginSettings _loginSettings;

        public ApiAdminController(RmContext context, IOptions<RmSettings> options, IOptions<RmLoginSettings> loginOptions) : base(context, options)
        {
            _loginSettings = loginOptions.Value;
        }

        [HttpGet("updatecoursedata")]
        public IActionResult UpdateCourseData()
        {
            CourseData.UpdateCourses(_context);

            return Json("success");
        }

        #region Pages

        private class AdminPageBrief
        {
            public int PageId { get; set; }
            public string Path { get; set; }
            public string Title { get; set; }
            public bool Enabled { get; set; }
        }

        [HttpGet("pages")]
        public IActionResult Pages()
        {
            var query = from p in _context.Pages
                orderby p.PageId descending
                select new AdminPageBrief
                {
                    PageId = p.PageId,
                    Path = p.Path,
                    Title = p.Title,
                    Enabled = p.Enabled
                };

            return Json(query.ToList());
        }

        [HttpGet("page")]
        public IActionResult Page(int id)
        {
            var page = _context.Pages.FirstOrDefault(p => p.PageId == id);
            return page == null ? Json("page not found") : Json(page);
        }

        [HttpPost("newpage")]
        public IActionResult NewPage()
        {
            try
            {
                var time = DateTime.Now;
                var page = new Page
                {
                    CommentEntryNumber = _context.NewCommentEntryNumber(),
                    CreateTime = time,
                    ModifyTime = time
                };

                _context.Pages.Add(page);
                _context.SaveChanges();

                return Json(page.PageId);
            }
            catch (Exception)
            {
                // TODO: log
                return Json("failed");
            }
        }

        [HttpPost("updatepage")]
        public IActionResult UpdatePage(Page np)
        {
            var page = _context.Pages.FirstOrDefault(p => p.PageId == np.PageId);
            if (page == null)
                return Json("page not found");

            try
            {
                page.Path = np.Path;
                page.Enabled = np.Enabled;
                page.RequireLogin = np.RequireLogin;
                page.RequireFullMember = np.RequireFullMember;
                page.RequireAdmin = np.RequireAdmin;
                page.Title = np.Title;
                page.Subtitle = np.Subtitle;
                page.Content = np.Content;
                page.RawContent = np.RawContent;
                page.JavaScriptFiles = np.JavaScriptFiles;
                page.CssFiles = np.CssFiles;
                page.HomeOrder = np.HomeOrder;
                page.SplashOrder = np.SplashOrder;
                page.NavbarOrder = np.NavbarOrder;
                page.ThumbnailImage = np.ThumbnailImage;
                page.SplashImage = np.SplashImage;
                page.ModifyTime = DateTime.Now;

                _context.SaveChanges();

                return Json("success");
            }
            catch (Exception)
            {
                // TODO: log
                return Json("failed");
            }
        }

        [HttpDelete("deletepage")]
        public IActionResult DeletePage(int id)
        {
            var page = _context.Pages.FirstOrDefault(p => p.PageId == id);
            if (page == null)
                return Json("page not found");

            try
            {
                _context.Pages.Remove(page);
                _context.SaveChanges();
                _context.DisableCommentEntry(page.CommentEntryNumber);

                return Json("success");
            }
            catch (Exception)
            {
                // TODO: log
                return Json("failed");
            }
        }

        #endregion

        #region User Login

        [HttpPost("createuserlogin")]
        public IActionResult CreateUserLogin(int userId, string name, string pwdhash)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return Json("user not found");

                UserManager.CreateLoginForUser(user, name, pwdhash, _loginSettings, _context);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        #endregion
    }
}
