using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class Page
    {
        public int PageId { get; set; }
        public int CommentEntryNumber { get; set; }

        public string Path { get; set; }
        public bool Enabled { get; set; }
        public bool RequireLogin { get; set; }
        public bool RequireFullMember { get; set; }
        public bool RequireAdmin { get; set; }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public bool RawContent { get; set; }
        public string JavaScriptFiles { get; set; }
        public string CssFiles { get; set; }

        public int HomeOrder { get; set; }
        public int SplashOrder { get; set; }
        public int NavbarOrder { get; set; }

        public string ThumbnailImage { get; set; }
        public string SplashImage { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
