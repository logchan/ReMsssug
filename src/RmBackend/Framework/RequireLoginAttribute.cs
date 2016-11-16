using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RmBackend.Access;

namespace RmBackend.Framework
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public bool RequireAdmin { get; set; } = false;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ctrl = context.Controller as Controller;

            if (ctrl != null)
            {
                var session = ctrl.HttpContext.Session;
                if (!UserManager.IsLoggedIn(session) ||
                    (RequireAdmin && !UserManager.IsAdmin(session)))
                {
                    context.Result = ctrl.Unauthorized();
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
