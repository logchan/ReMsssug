using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmBackend.Utilities
{
    public static class Extensions
    {
        public static string GetExceptionString(this Exception ex, string source, string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{source}] {title}");
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);

            return sb.ToString();
        }
    }
}
