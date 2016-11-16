using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Access;
using RmBackend.Framework;
using RmBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace RmBackend.Controllers.Api
{
    [Route("api/cwiki")]
    public class ApiCwikiController : RmApiControllerBase
    {
        public ApiCwikiController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        private class ReviewBrief
        {
            public int CourseReviewId { get; set; }
            public Course Course { get; set; }
            public string Title { get; set; }
            public PostStatus Status { get; set; }
        }

        [HttpGet("courses")]
        public IActionResult GetCoursesWithReviews()
        {
            var query = from c in _context.Courses
                        where _context.CourseReviews.Any(
                            r => r.CourseId == c.CourseId && 
                        (r.Status == PostStatus.Posted || 
                        r.Status == PostStatus.NeedApproval || 
                        r.Status == PostStatus.NeedModification))
                        select c;

            return Json(query.ToList());
        }

        [HttpGet("allcourses")]
        public IActionResult GetAllCourses()
        {
            return Json(_context.Courses.ToList());
        }

        [HttpGet("latestreviews")]
        public IActionResult GetLatestReviews()
        {
            var query = from r in _context.CourseReviews
                where r.Status == PostStatus.Posted
                orderby r.ModifyTime descending
                select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title};

            return Json(query.Take(10).ToList());
        }

        [HttpGet("latestcomment")]
        public IActionResult GetLatestComment()
        {
            var cquery = from c in _context.Comments
                         where _context.CourseReviews.Any(r => c.EntryNumber == r.CommentEntryNumber)
                         orderby c.ModifyTime descending
                         select c.EntryNumber;

            var entries = cquery.Take(10).ToList();
            var rquery = from r in _context.CourseReviews
                         where entries.Contains(r.CommentEntryNumber)
                         orderby entries.IndexOf(r.CommentEntryNumber)
                         select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title };

            return Json(rquery.ToList());
        }

        [HttpGet("review")]
        public IActionResult GetReview(int reviewId)
        {
            var review = _context.CourseReviews.Include(r => r.Course).Include(r => r.User).FirstOrDefault(r => r.CourseReviewId == reviewId);
            if (review == null || review.Status == PostStatus.Deleted)
            {
                return Json("review not found");
            }

            if (review.Status != PostStatus.Posted)
            {
                var user = UserManager.GetUser(HttpContext.Session);
                if (user == null || user.UserId != review.UserId)
                {
                    return Json("review not public");
                }
            }

            return Json(review);
        }

        [HttpGet("course")]
        public IActionResult GetByCourse(string param)
        {
            param = param?.Trim();
            if (String.IsNullOrEmpty(param))
                return Json("[]");

            int id;
            if (Int32.TryParse(param, out id))
            {
                var query = from r in _context.CourseReviews
                    where r.CourseId == id && r.Status == PostStatus.Posted
                    select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title };
                return Json(query.ToList());
            }
            else
            {
                var query = from r in _context.CourseReviews
                    where r.Course.Code == param && r.Status == PostStatus.Posted
                    select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title };
                return Json(query.ToList());
            }
        }

        [HttpGet("user")]
        public IActionResult GetByUser(string param)
        {
            param = param?.Trim();
            if (String.IsNullOrEmpty(param))
                return Json("[]");

            int id;
            User user = Int32.TryParse(param, out id) ? 
                _context.Users.FirstOrDefault(u => u.UserId == id) : 
                _context.Users.FirstOrDefault(u => u.Itsc == param);

            var currentUser = UserManager.GetUser(HttpContext.Session);
            if (currentUser != null && currentUser.UserId == user.UserId)
            {
                var query = from r in _context.CourseReviews
                    where r.UserId == user.UserId
                    select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title };
                return Json(query.ToList());
            }
            else
            {
                var query = from r in _context.CourseReviews
                    where r.UserId == user.UserId && r.Status == PostStatus.Posted
                    select new ReviewBrief { Course = r.Course, CourseReviewId = r.CourseReviewId, Status = r.Status, Title = r.Title };
                return Json(query.ToList());
            }
        }

        private bool IsValidReview(string title, string content)
        {
            if (String.IsNullOrWhiteSpace(title) || title.Length > 20)
                return false;

            if (String.IsNullOrWhiteSpace(content))
                return false;

            return true;
        }

        private void AddVersionLog(CourseReview review)
        {
            var sb = new StringBuilder(review.VersionLog);
            sb.AppendLine("<!--logentry start-->");
            sb.AppendLine("### TIME");
            sb.AppendLine();
            sb.AppendLine(review.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("### TITLE");
            sb.AppendLine();
            sb.AppendLine(review.Title);
            sb.AppendLine();
            sb.AppendLine("### CONTENT");
            sb.AppendLine();
            sb.AppendLine(review.Content);
            sb.AppendLine();
            sb.AppendLine("<!--logentry end-->");
            review.VersionLog = sb.ToString();
        }

        [HttpPost("add")]
        [RequireLogin]
        public IActionResult Add(int courseId, string title, string content)
        {
            if (!IsValidReview(title, content))
            {
                return Json("review invalid");
            }

            var user = UserManager.GetUser(HttpContext.Session);

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                return Json("no such course");
            }

            var existing = _context.CourseReviews.FirstOrDefault(r => r.CourseId == courseId && r.UserId == user.UserId);
            if (existing != null)
            {
                return Json("review exists");
            }

            try
            {
                var time = DateTime.Now;
                var review = new CourseReview
                {
                    CourseId = courseId,
                    Title = title,
                    Content = content,
                    CreateTime = time,
                    ModifyTime = time,
                    Status = PostStatus.Posted,
                    UserId = user.UserId,
                    VersionLog = "",
                    CommentEntryNumber = _context.NewCommentEntryNumber()
                };

                _context.CourseReviews.Add(review);
                _context.SaveChanges();

                return Json(review.CourseReviewId);
            }
            catch (Exception)
            {
                // TODO: log exception
                return Json("failed");
            }
        }

        [HttpPost("update")]
        [RequireLogin]
        public IActionResult Update(int reviewId, string title, string content)
        {
            if (!IsValidReview(title, content))
            {
                return Json("review invalid");
            }

            var user = UserManager.GetUser(HttpContext.Session);

            var review = _context.CourseReviews.FirstOrDefault(c => c.CourseReviewId == reviewId);
            if (review == null || review.Status == PostStatus.Deleted)
            {
                return Json("review not found");
            }

            if (review.UserId != user.UserId)
            {
                return Json("not your review");
            }

            try
            {
                AddVersionLog(review);
                review.Title = title;
                review.Content = content;
                review.ModifyTime = DateTime.Now;
                if (review.Status == PostStatus.NeedModification)
                    review.Status = PostStatus.NeedApproval;

                _context.SaveChanges();
            }
            catch (Exception)
            {
                // TODO: log
                return Json("failed");
            }

            return Json(reviewId);
        }

        [HttpDelete("delete")]
        [RequireLogin]
        public IActionResult Delete(int reviewId)
        {
            var user = UserManager.GetUser(HttpContext.Session);

            var review = _context.CourseReviews.FirstOrDefault(c => c.CourseReviewId == reviewId);
            if (review == null || review.Status == PostStatus.Deleted)
            {
                return Json("review not found");
            }

            if (review.UserId != user.UserId)
            {
                return Json("not your review");
            }

            try
            {
                review.Status = PostStatus.Deleted;
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // TODO: log
                return Json("failed");
            }

            return Json("success");
        }
    }
}
