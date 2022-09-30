using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedFieldsInNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Media_UserImageId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserImageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "MessageArchived",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserImageId",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Notifications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserImage",
                table: "Notifications",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Customers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Customers_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MessageArchived",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserImageId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserImageId",
                table: "Notifications",
                column: "UserImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Media_UserImageId",
                table: "Notifications",
                column: "UserImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
