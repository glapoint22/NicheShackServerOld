using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ChangedDeleteBehaviorOnNotificationGroupTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes",
                column: "NotificationGroupId",
                principalTable: "NotificationGroups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes",
                column: "NotificationGroupId",
                principalTable: "NotificationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
