using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RmBackend.Access;
using RmBackend.Models;
using RmBackend.Framework;

namespace RmBackend.Controllers.Api
{
    [Route("api/comment")]
    public class ApiCommentController : RmApiControllerBase
    {
        public ApiCommentController(RmContext context, IOptions<RmSettings> options) : base(context, options)
        {
        }

        public class CommentData
        {
            public int CommentId { get; set; }
            public int EntryNumber { get; set; }
            public bool IsAnonymous { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime ModifyTime { get; set; }
            public int? ParentId { get; set; }
            public User User { get; set; }
        }

        [HttpGet("get")]
        public IActionResult GetComments(int entryId)
        {
            var entry = _context.CommentEntries.FirstOrDefault(e => e.CommentEntryId == entryId);
            if (entry == null)
            {
                return Json("invalid comment entry");
            }

            if (entry.Disabled)
            {
                return Json("comment disabled");
            }

            var userId = UserManager.GetUser(HttpContext.Session)?.UserId ?? -1;
            var query = from c in _context.Comments
                        where c.EntryNumber == entryId
                        select new CommentData
                        {
                            CommentId = c.CommentId,
                            EntryNumber = c.EntryNumber,
                            IsAnonymous = c.IsAnonymous,
                            Title = c.Title,
                            Content = c.Status == PostStatus.Posted ? c.Content : "[hidden or removed]",
                            CreateTime = c.CreateTime,
                            ModifyTime = c.ModifyTime,
                            ParentId = c.ParentId,
                            // hide user if IsAnonymous and (not logged in or not author)
                            User = c.IsAnonymous && c.UserId != userId ? null : c.User
                        };

            var list = query.ToList();
            if (!entry.AllowPost)
            {
                list.Add(null); // use null entry to indicate comment closed
            }

            return Json(list);
        }

        [HttpPost("post")]
        [RequireLogin]
        public IActionResult PostComment(CommentData data)
        {
            var entry = _context.CommentEntries.FirstOrDefault(e => e.CommentEntryId == data.EntryNumber);
            if (entry == null || entry.Disabled || !entry.AllowPost)
            {
                return Json("invalid comment entry");
            }

            try
            {
                var time = DateTime.Now;
                var user = UserManager.GetUser(HttpContext.Session);
                var comment = new Comment
                {
                    Status = PostStatus.Posted,
                    EntryNumber = data.EntryNumber,
                    IsAnonymous = data.IsAnonymous,
                    Title = data.Title,
                    Content = data.Content,
                    CreateTime = time,
                    ModifyTime = time,
                    ParentId = data.ParentId,
                    UserId = user.UserId,
                    VersionLog = ""
                };
                _context.Comments.Add(comment);
                _context.SaveChanges();

                return Json("success");
            }
            catch (Exception)
            {
                //TODO: log
                return Json("failed");
            }
        }

        // TODO: refactor this :p
        private void AddVersionLog(Comment comment)
        {
            var sb = new StringBuilder(comment.VersionLog);
            sb.AppendLine("<!--logentry start-->");
            sb.AppendLine("### TIME");
            sb.AppendLine();
            sb.AppendLine(comment.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("### TITLE");
            sb.AppendLine();
            sb.AppendLine(comment.Title);
            sb.AppendLine();
            sb.AppendLine("### CONTENT");
            sb.AppendLine();
            sb.AppendLine(comment.Content);
            sb.AppendLine();
            sb.AppendLine("<!--logentry end-->");
            comment.VersionLog = sb.ToString();
        }

        [HttpPost("update")]
        [RequireLogin]
        public IActionResult UpdateComment(CommentData data)
        {
            var entry = _context.CommentEntries.FirstOrDefault(e => e.CommentEntryId == data.EntryNumber);
            if (entry == null || entry.Disabled || !entry.AllowPost)
            {
                return Json("invalid comment entry");
            }

            var comment = _context.Comments.FirstOrDefault(
                c => c.CommentId == data.CommentId && c.Status == PostStatus.Posted);
            if (comment == null)
            {
                return Json("comment not found");
            }

            var user = UserManager.GetUser(HttpContext.Session);
            if (user.UserId != comment.UserId)
            {
                return Json("not your comment");
            }

            try
            {
                AddVersionLog(comment);

                comment.IsAnonymous = data.IsAnonymous;
                comment.Title = data.Title;
                comment.Content = data.Content;
                comment.ModifyTime = DateTime.Now;
                
                _context.SaveChanges();

                return Json("success");
            }
            catch (Exception)
            {
                //TODO: log
                return Json("failed");
            }
        }

        [HttpDelete("delete")]
        [RequireLogin]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == id && c.Status == PostStatus.Posted);
            if (comment == null)
            {
                return Json("comment not found");
            }

            var entry = _context.CommentEntries.FirstOrDefault(e => e.CommentEntryId == comment.EntryNumber);
            if (entry == null || entry.Disabled || !entry.AllowPost)
            {
                return Json("invalid comment entry");
            }

            var user = UserManager.GetUser(HttpContext.Session);
            if (user.UserId != comment.UserId)
            {
                return Json("not your comment");
            }

            try
            {
                comment.Status = PostStatus.Deleted;
                _context.SaveChanges();

                return Json("success");
            }
            catch (Exception)
            {
                //TODO: log
                return Json("failed");
            }
        }
    }
}
