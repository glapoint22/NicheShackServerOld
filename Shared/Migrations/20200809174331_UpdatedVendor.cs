using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedVendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Emails");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Emails",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Emails");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Emails",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
