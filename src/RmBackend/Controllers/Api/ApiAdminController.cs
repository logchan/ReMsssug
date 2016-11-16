using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Data;
using RmBackend.Framework;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
    [Route("api/admin")]
    [RequireLogin(RequireAdmin = true)]
    public class ApiAdminController : RmApiControllerBase
    {
        public ApiAdminController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        [HttpGet("updatecoursedata")]
        public IActionResult UpdateCourseData()
        {
            CourseData.UpdateCourses(_context);

            return Json("success");
        }
    }
}
