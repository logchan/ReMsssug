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
    }
}
