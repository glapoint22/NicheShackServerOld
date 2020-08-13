using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedEmails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "ProductEmails");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "LeadPageEmails");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductEmails",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LeadPageEmails",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductEmails");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LeadPageEmails");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "ProductEmails",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "LeadPageEmails",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
