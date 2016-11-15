using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class CourseReview
    {
        public int CourseReviewId { get; set; }
        public int CommentEntryNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public PostStatus Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public string VersionLog { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
