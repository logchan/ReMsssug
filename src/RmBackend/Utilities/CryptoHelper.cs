using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RmBackend.Utilities
{
    public static class CryptoHelper
    {
        private static MD5 _md5 = MD5.Create();

        public static string GetMd5String(string input)
        {
            var data = _md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
