using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Collaborators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddToList",
                table: "ListCollaborators",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeleteList",
                table: "ListCollaborators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EditList",
                table: "ListCollaborators",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "InviteCollaborators",
                table: "ListCollaborators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MoveItem",
                table: "ListCollaborators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RemoveItem",
                table: "ListCollaborators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShareList",
                table: "ListCollaborators",
                nullable: true,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddToList",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "DeleteList",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "EditList",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "InviteCollaborators",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "MoveItem",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "RemoveItem",
                table: "ListCollaborators");

            migrationBuilder.DropColumn(
                name: "ShareList",
                table: "ListCollaborators");
        }
    }
}
