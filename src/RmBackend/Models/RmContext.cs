using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RmBackend.Models
{
    public class RmContext : DbContext
    {
        public RmContext(DbContextOptions<RmContext> options)
            : base(options)
        {

        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<CommentEntry> CommentEntries { get; set; }

        public int NewCommentEntryNumber()
        {
            var entry = new CommentEntry
            {
                AllowPost = true,
                Disabled = false
            };
            CommentEntries.Add(entry);
            SaveChanges();

            return entry.CommentEntryId;
        }

        public void DisableCommentEntry(int id)
        {
            var entry = CommentEntries.FirstOrDefault(e => e.CommentEntryId == id);
            if (entry != null)
            {
                entry.Disabled = true;
                SaveChanges();
            }
        }
    }
}
