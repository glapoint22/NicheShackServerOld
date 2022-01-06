using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class NewMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Video",
                table: "Media");

            migrationBuilder.AddColumn<int>(
                name: "MediaType",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoId",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VideoType",
                table: "Media",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "VideoType",
                table: "Media");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Media",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "Media",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
