using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedNonAccountFieldsInNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NonAccountUserEmail",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NonAccountUserName",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "NonAccountEmail",
                table: "Notifications",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonAccountName",
                table: "Notifications",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NonAccountEmail",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NonAccountName",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "NonAccountUserEmail",
                table: "Notifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonAccountUserName",
                table: "Notifications",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
