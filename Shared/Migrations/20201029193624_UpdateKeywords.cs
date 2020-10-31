using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateKeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductKeywords");

            migrationBuilder.AddColumn<int>(
                name: "KeywordId",
                table: "ProductKeywords",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeywordSearchVolumes",
                columns: table => new
                {
                    KeywordId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordSearchVolumes", x => new { x.KeywordId, x.Date });
                    table.ForeignKey(
                        name: "FK_KeywordSearchVolumes_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords",
                column: "KeywordId");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_Name",
                table: "Keywords",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductKeywords_Keywords_KeywordId",
                table: "ProductKeywords",
                column: "KeywordId",
                principalTable: "Keywords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductKeywords_Keywords_KeywordId",
                table: "ProductKeywords");

            migrationBuilder.DropTable(
                name: "KeywordSearchVolumes");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords");

            migrationBuilder.DropColumn(
                name: "KeywordId",
                table: "ProductKeywords");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductKeywords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
