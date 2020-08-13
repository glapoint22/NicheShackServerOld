using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddedURLFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlId",
                table: "Niches",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlName",
                table: "Niches",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlId",
                table: "Categories",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlName",
                table: "Categories",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlId",
                table: "Niches");

            migrationBuilder.DropColumn(
                name: "UrlName",
                table: "Niches");

            migrationBuilder.DropColumn(
                name: "UrlId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UrlName",
                table: "Categories");
        }
    }
}
