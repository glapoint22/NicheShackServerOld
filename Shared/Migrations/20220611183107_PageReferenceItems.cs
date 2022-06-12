using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class PageReferenceItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PageReferenceItems_ItemId",
                table: "PageReferenceItems");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "PageReferenceItems");

            migrationBuilder.AddColumn<int>(
                name: "KeywordGroupId",
                table: "PageReferenceItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NicheId",
                table: "PageReferenceItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageReferenceItems_KeywordGroupId",
                table: "PageReferenceItems",
                column: "KeywordGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PageReferenceItems_NicheId",
                table: "PageReferenceItems",
                column: "NicheId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageReferenceItems_KeywordGroups_KeywordGroupId",
                table: "PageReferenceItems",
                column: "KeywordGroupId",
                principalTable: "KeywordGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageReferenceItems_Niches_NicheId",
                table: "PageReferenceItems",
                column: "NicheId",
                principalTable: "Niches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageReferenceItems_KeywordGroups_KeywordGroupId",
                table: "PageReferenceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PageReferenceItems_Niches_NicheId",
                table: "PageReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_PageReferenceItems_KeywordGroupId",
                table: "PageReferenceItems");

            migrationBuilder.DropIndex(
                name: "IX_PageReferenceItems_NicheId",
                table: "PageReferenceItems");

            migrationBuilder.DropColumn(
                name: "KeywordGroupId",
                table: "PageReferenceItems");

            migrationBuilder.DropColumn(
                name: "NicheId",
                table: "PageReferenceItems");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "PageReferenceItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PageReferenceItems_ItemId",
                table: "PageReferenceItems",
                column: "ItemId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "PageId" });
        }
    }
}
