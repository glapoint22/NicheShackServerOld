using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdateNotificationText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NotificationText",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NotificationText",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "NotificationText");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "NotificationText");
        }
    }
}
