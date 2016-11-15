using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Access;
using RmBackend.Data;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
#if DEBUG
    [Route("debug")]
    public class ApiDebugController : RmApiControllerBase
    {
        public ApiDebugController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        [HttpGet("initdata")]
        public IActionResult InitData()
        {
            var user = _context.Users.FirstOrDefault(u => u.Itsc == "localadmin");
            if (user == null)
            {
                user = new User
                {
                    Itsc = "localadmin",
                    Nickname = "local admin"
                };
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            UserManager.AssignUser(HttpContext.Session, user);

            if (!_context.Courses.Any())
            {
                CourseData.UpdateCourses(_context);
            }

            return Json("success");
        }
    }
#endif
}
