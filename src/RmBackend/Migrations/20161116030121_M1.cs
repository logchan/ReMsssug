using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RmBackend.Migrations
{
    public partial class M1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    PageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentEntryNumber = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    CssFiles = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    HomeOrder = table.Column<int>(nullable: false),
                    JavaScriptFiles = table.Column<string>(nullable: true),
                    ModifyTime = table.Column<DateTime>(nullable: false),
                    NavbarOrder = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    RawContent = table.Column<bool>(nullable: false),
                    RequireAdmin = table.Column<bool>(nullable: false),
                    RequireFullMember = table.Column<bool>(nullable: false),
                    RequireLogin = table.Column<bool>(nullable: false),
                    SplashImage = table.Column<string>(nullable: true),
                    SplashOrder = table.Column<int>(nullable: false),
                    Subtitle = table.Column<string>(nullable: true),
                    ThumbnailImage = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.PageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pages");
        }
    }
}
