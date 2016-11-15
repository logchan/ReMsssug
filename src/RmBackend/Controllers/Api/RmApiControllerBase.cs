using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Models;

namespace RmBackend.Controllers.Api
{
    public class RmApiControllerBase : Controller
    {
        protected RmContext _context;
        protected RmSettings _settings;

        public RmApiControllerBase(RmContext context, IOptions<RmSettings> options)
        {
            _context = context;
            _settings = options.Value;
        }
    }
}
