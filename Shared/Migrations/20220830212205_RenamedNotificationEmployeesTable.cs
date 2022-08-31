using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedNotificationEmployeesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_NotificationEmployees_NotificationEmployeeId",
                table: "NotificationDetails");

            migrationBuilder.DropTable(
                name: "NotificationEmployees");

            migrationBuilder.CreateTable(
                name: "NotificationEmployeeDetails",
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
                    table.PrimaryKey("PK_NotificationEmployeeDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmployeeDetails_Customers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_NotificationEmployeeDetails_NotificationEmployeeId",
                table: "NotificationDetails");

            migrationBuilder.DropTable(
                name: "NotificationEmployeeDetails");

            migrationBuilder.CreateTable(
                name: "NotificationEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmployees_Customers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmployees_EmployeeId",
                table: "NotificationEmployees",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDetails_NotificationEmployees_NotificationEmployeeId",
                table: "NotificationDetails",
                column: "NotificationEmployeeId",
                principalTable: "NotificationEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
