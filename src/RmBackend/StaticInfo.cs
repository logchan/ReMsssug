using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RmBackend
{
    public static class StaticInfo
    {
        static StaticInfo()
        {
            JsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public static JsonSerializerSettings JsonSettings { get; private set; }
    }
}
