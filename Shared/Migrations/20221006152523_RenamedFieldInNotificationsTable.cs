using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedFieldInNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserComment",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Notifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "UserComment",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
