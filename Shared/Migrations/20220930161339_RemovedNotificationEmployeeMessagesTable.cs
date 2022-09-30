using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RemovedNotificationEmployeeMessagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeMessageId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationEmployeeMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
    }
}
