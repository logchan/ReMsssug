using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int EntryNumber { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public string Title { get; set; }
        public string Content { get; set; }

        public PostStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public string VersionLog { get; set; }

        public int? ParentId { get; set; }
        public Comment Parent { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
