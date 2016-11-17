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
        private RmLoginSettings _loginSettings;

        public ApiUserController(RmContext context, IOptions<RmSettings> options, IOptions<RmLoginSettings> loginOptions) : base(context, options)
        {
            _loginSettings = loginOptions.Value;
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

        [HttpPost("login")]
        public IActionResult Login(string username, string pwdhash)
        {
            var accepted = UserManager.LoginWithCredentials(HttpContext.Session, username, pwdhash, _loginSettings, _context);

            return Json(accepted);
        }

        [HttpPost("login3")]
        public IActionResult ThirdPartyLogin(string itsc, string time, string hash)
        {
            var token = UserManager.ThirdPartyLogin(itsc, time, hash, _loginSettings, _context);

            return Json(token);
        }

        [HttpPost("redeem")]
        public IActionResult RedeemToken(string token)
        {
            return Json(UserManager.RedeemToken(HttpContext.Session, token, _context));
        }
    }
}
