using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class UserLogin
    {
        public int UserLoginId { get; set; }

        public string Name { get; set; }
        public string Hash { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
