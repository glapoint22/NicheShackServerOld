using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Image200x200",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image500x500",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAnySize",
                table: "Media",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image200x200",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Image500x500",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageAnySize",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Media",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
