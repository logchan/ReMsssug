using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using RmBackend.Models;

namespace RmBackend.Migrations
{
    [DbContext(typeof(RmContext))]
    [Migration("20161116085750_M2")]
    partial class M2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RmBackend.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime");

                    b.Property<int>("EntryNumber");

                    b.Property<bool>("IsAnonymous");

                    b.Property<DateTime>("ModifyTime");

                    b.Property<int?>("ParentId");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("UserId");

                    b.Property<string>("VersionLog");

                    b.HasKey("CommentId");

                    b.HasIndex("ParentId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("RmBackend.Models.CommentEntry", b =>
                {
                    b.Property<int>("CommentEntryId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowPost");

                    b.Property<bool>("Disabled");

                    b.HasKey("CommentEntryId");

                    b.ToTable("CommentEntries");
                });

            modelBuilder.Entity("RmBackend.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasColumnName("CourseCode");

                    b.Property<string>("Name")
                        .HasColumnName("CourseName");

                    b.HasKey("CourseId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("RmBackend.Models.CourseReview", b =>
                {
                    b.Property<int>("CourseReviewId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommentEntryNumber");

                    b.Property<string>("Content");

                    b.Property<int>("CourseId");

                    b.Property<DateTime>("CreateTime");

                    b.Property<DateTime>("ModifyTime");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("UserId");

                    b.Property<string>("VersionLog");

                    b.HasKey("CourseReviewId");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("CourseReviews");
                });

            modelBuilder.Entity("RmBackend.Models.Page", b =>
                {
                    b.Property<int>("PageId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CommentEntryNumber");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("CssFiles");

                    b.Property<bool>("Enabled");

                    b.Property<int>("HomeOrder");

                    b.Property<string>("JavaScriptFiles");

                    b.Property<DateTime>("ModifyTime");

                    b.Property<int>("NavbarOrder");

                    b.Property<string>("Path");

                    b.Property<bool>("RawContent");

                    b.Property<bool>("RequireAdmin");

                    b.Property<bool>("RequireFullMember");

                    b.Property<bool>("RequireLogin");

                    b.Property<string>("SplashImage");

                    b.Property<int>("SplashOrder");

                    b.Property<string>("Subtitle");

                    b.Property<string>("ThumbnailImage");

                    b.Property<string>("Title");

                    b.HasKey("PageId");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("RmBackend.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Itsc");

                    b.Property<string>("Nickname");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RmBackend.Models.Comment", b =>
                {
                    b.HasOne("RmBackend.Models.Comment", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("RmBackend.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RmBackend.Models.CourseReview", b =>
                {
                    b.HasOne("RmBackend.Models.Course", "Course")
                        .WithMany("Reviews")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RmBackend.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
