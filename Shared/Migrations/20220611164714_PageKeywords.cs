using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class PageKeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Keywords_In_KeywordGroup",
                table: "Keywords_In_KeywordGroup");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Keywords_In_KeywordGroup",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Keywords_In_KeywordGroup",
                table: "Keywords_In_KeywordGroup",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PageKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageId = table.Column<int>(nullable: false),
                    KeywordInKeywordGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageKeywords_Keywords_In_KeywordGroup_KeywordInKeywordGroupId",
                        column: x => x.KeywordInKeywordGroupId,
                        principalTable: "Keywords_In_KeywordGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageKeywords_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_In_KeywordGroup_KeywordGroupId",
                table: "Keywords_In_KeywordGroup",
                column: "KeywordGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PageKeywords_KeywordInKeywordGroupId",
                table: "PageKeywords",
                column: "KeywordInKeywordGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PageKeywords_PageId",
                table: "PageKeywords",
                column: "PageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageKeywords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Keywords_In_KeywordGroup",
                table: "Keywords_In_KeywordGroup");

            migrationBuilder.DropIndex(
                name: "IX_Keywords_In_KeywordGroup_KeywordGroupId",
                table: "Keywords_In_KeywordGroup");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Keywords_In_KeywordGroup");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Keywords_In_KeywordGroup",
                table: "Keywords_In_KeywordGroup",
                columns: new[] { "KeywordGroupId", "KeywordId" });
        }
    }
}
