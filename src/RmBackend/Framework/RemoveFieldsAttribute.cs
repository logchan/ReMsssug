using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RmBackend.Access;
using System.Reflection;

namespace RmBackend.Framework
{
    public class RemoveFieldsAttribute : ActionFilterAttribute
    {
        private List<string> _fieldsToRemove;

        public bool AdminBypass { get; set; }

        public RemoveFieldsAttribute(params string[] fields)
        {
            _fieldsToRemove = fields.ToList();
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!AdminBypass ||
                !UserManager.IsAdmin(context.HttpContext.Session))
            {
                var result = context.Result as JsonResult;

                if (result == null)
                    goto end;

                var type = result.Value.GetType();
                var typeInfo = type.GetTypeInfo();
                var isList = false;
                var ienum = result.Value as System.Collections.IEnumerable;
                if (typeInfo.IsGenericType && ienum != null)
                {
                    isList = true;
                    type = type.GetGenericArguments()[0];
                }

                foreach (var field in _fieldsToRemove)
                {
                    var p = type.GetProperty(field);
                    if (p != null && p.PropertyType.GetTypeInfo().IsClass)
                    {
                        if (!isList)
                            p.SetValue(result.Value, null);
                        else
                        {
                            foreach (object o in ienum)
                            {
                                p.SetValue(o, null);
                            }
                        }
                    }
                }
            }

            end:
            base.OnResultExecuting(context);
        }
    }

}
