using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RmBackend.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("processlogin")]
        public IActionResult ProcessLogin(string time, string hash, string itsc)
        {
            ViewBag.Time = time ?? "";
            ViewBag.Hash = hash ?? "";
            ViewBag.Itsc = itsc ?? "";
            return View();
        }
    }
}
