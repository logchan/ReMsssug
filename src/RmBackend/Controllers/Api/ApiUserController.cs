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
    [Route("api/user")]
    public class ApiUserController : RmApiControllerBase
    {
        public ApiUserController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        [HttpGet("current")]
        public IActionResult CurrentUser()
        {
            var user = UserManager.GetUser(HttpContext.Session);
            if (user == null)
                return Json("not logged in");

            return Json(new {user.UserId, user.Nickname, user.Itsc});
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            UserManager.Logout(HttpContext.Session);

            return Json("success");
        }
    }
}
