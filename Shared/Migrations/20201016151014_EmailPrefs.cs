using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class EmailPrefs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefAddedListItem",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefDeletedList",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefEmailChange",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefListNameChange",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefMovedListItem",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefNameChange",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefNewCollaborator",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefPasswordChange",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefProfilePicChange",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefRemovedCollaborator",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefRemovedListItem",
                table: "Customers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailPrefReview",
                table: "Customers",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailPrefAddedListItem",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefDeletedList",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefEmailChange",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefListNameChange",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefMovedListItem",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefNameChange",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefNewCollaborator",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefPasswordChange",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefProfilePicChange",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefRemovedCollaborator",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefRemovedListItem",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailPrefReview",
                table: "Customers");
        }
    }
}
