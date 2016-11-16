using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RmBackend.Framework;

namespace RmBackend.Controllers
{
    [Route("admin")]
    [RequireLogin(RequireAdmin = true)]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("pages")]
        public IActionResult Pages()
        {
            return View();
        }

        [HttpGet("updatepage")]
        public IActionResult UpdatePage()
        {
            return View();
        }
    }
}
