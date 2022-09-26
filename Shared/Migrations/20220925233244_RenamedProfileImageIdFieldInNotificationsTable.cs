using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedProfileImageIdFieldInNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Media_ProfileImageId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ProfileImageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "UserImageId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserImageId",
                table: "Notifications",
                column: "UserImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Media_UserImageId",
                table: "Notifications",
                column: "UserImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Media_UserImageId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserImageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserImageId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "ProfileImageId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProfileImageId",
                table: "Notifications",
                column: "ProfileImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Media_ProfileImageId",
                table: "Notifications",
                column: "ProfileImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
