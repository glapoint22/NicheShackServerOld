using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddedNotificationEmployeeMessagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeMessageId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationEmployeeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmployeeMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmployeeMessages_Customers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EmployeeMessageId",
                table: "Notifications",
                column: "EmployeeMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmployeeMessages_EmployeeId",
                table: "NotificationEmployeeMessages",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationEmployeeMessages_EmployeeMessageId",
                table: "Notifications",
                column: "EmployeeMessageId",
                principalTable: "NotificationEmployeeMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationEmployeeMessages_EmployeeMessageId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationEmployeeMessages");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_EmployeeMessageId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EmployeeMessageId",
                table: "Notifications");
        }
    }
}
