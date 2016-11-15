using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
    [Route("api/cwiki")]
    public class ApiCwikiController : RmApiControllerBase
    {
        public ApiCwikiController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }
    }
}
