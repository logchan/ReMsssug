using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RmBackend.Controllers
{
    [Route("page")]
    public class PageController : Controller
    {
        [HttpGet("{*path}")]
        public IActionResult Page(string path)
        {
            ViewBag.Path = path;
            return View();
        }
    }
}
