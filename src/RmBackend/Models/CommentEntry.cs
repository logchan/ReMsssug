using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class CommentEntry
    {
        public int CommentEntryId { get; set; }
        public bool Disabled { get; set; }
        public bool AllowPost { get; set; }
    }
}
