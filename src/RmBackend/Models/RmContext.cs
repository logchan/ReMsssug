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
    }
}
