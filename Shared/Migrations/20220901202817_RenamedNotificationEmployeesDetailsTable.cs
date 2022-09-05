using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedNotificationEmployeesDetailsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_NotificationEmployeeDetails_NotificationEmployeeId",
                table: "NotificationDetails");

            migrationBuilder.DropTable(
                name: "NotificationEmployeeDetails");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDetails_NotificationEmployeeId",
                table: "NotificationDetails");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationEmployeeId",
                table: "NotificationDetails");

            migrationBuilder.AddColumn<int>(
                name: "NotificationEmployeeNoteId",
                table: "NotificationDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationEmployeeNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmployeeNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmployeeNotes_Customers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationEmployeeNoteId",
                table: "NotificationDetails",
                column: "NotificationEmployeeNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmployeeNotes_EmployeeId",
                table: "NotificationEmployeeNotes",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDetails_NotificationEmployeeNotes_NotificationEmployeeNoteId",
                table: "NotificationDetails",
                column: "NotificationEmployeeNoteId",
                principalTable: "NotificationEmployeeNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_NotificationEmployeeNotes_NotificationEmployeeNoteId",
                table: "NotificationDetails");

            migrationBuilder.DropTable(
                name: "NotificationEmployeeNotes");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDetails_NotificationEmployeeNoteId",
                table: "NotificationDetails");

            migrationBuilder.DropColumn(
                name: "NotificationEmployeeNoteId",
                table: "NotificationDetails");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NotificationEmployeeId",
                table: "NotificationDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationEmployeeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmployeeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmployeeDetails_Customers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationEmployeeId",
                table: "NotificationDetails",
                column: "NotificationEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmployeeDetails_EmployeeId",
                table: "NotificationEmployeeDetails",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDetails_NotificationEmployeeDetails_NotificationEmployeeId",
                table: "NotificationDetails",
                column: "NotificationEmployeeId",
                principalTable: "NotificationEmployeeDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
