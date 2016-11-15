using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RmBackend.Controllers
{
    [Route("cwiki")]
    public class CwikiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("write")]
        public IActionResult Write()
        {
            return View();
        }

        [HttpGet("review")]
        public IActionResult Review()
        {
            return View();
        }

        [HttpGet("courselist")]
        public IActionResult CourseList()
        {
            return View();
        }

        [HttpGet("personal")]
        public IActionResult Personal()
        {
            return View();
        }

        [HttpGet("course")]
        public IActionResult Course()
        {
            return View();
        }
    }
}
