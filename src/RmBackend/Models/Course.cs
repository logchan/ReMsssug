using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RmBackend.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Column("CourseCode")]
        public string Code { get; set; }
        [Column("CourseName")]
        public string Name { get; set; }

        public List<CourseReview> Reviews { get; set; }
    }
}
