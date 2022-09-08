using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class CreatedNotificationGroupsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationDetails");

            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "NotificationEmployeeNotes");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "NotificationEmployeeNotes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Notifications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonAccountUserEmail",
                table: "Notifications",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NonAccountUserName",
                table: "Notifications",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationGroupId",
                table: "Notifications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserComment",
                table: "Notifications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "NotificationEmployeeNotes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "NotificationEmployeeNotes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationGroupId",
                table: "NotificationEmployeeNotes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NotificationGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationGroupId",
                table: "Notifications",
                column: "NotificationGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReviewId",
                table: "Notifications",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmployeeNotes_NotificationGroupId",
                table: "NotificationEmployeeNotes",
                column: "NotificationGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes",
                column: "NotificationGroupId",
                principalTable: "NotificationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationGroups_NotificationGroupId",
                table: "Notifications",
                column: "NotificationGroupId",
                principalTable: "NotificationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProductReviews_ReviewId",
                table: "Notifications",
                column: "ReviewId",
                principalTable: "ProductReviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationEmployeeNotes_NotificationGroups_NotificationGroupId",
                table: "NotificationEmployeeNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Customers_CustomerId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationGroups_NotificationGroupId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProductReviews_ReviewId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationGroups");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CustomerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationGroupId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ReviewId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_NotificationEmployeeNotes_NotificationGroupId",
                table: "NotificationEmployeeNotes");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NonAccountUserEmail",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NonAccountUserName",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationGroupId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserComment",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "NotificationEmployeeNotes");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "NotificationEmployeeNotes");

            migrationBuilder.DropColumn(
                name: "NotificationGroupId",
                table: "NotificationEmployeeNotes");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "NotificationEmployeeNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "NotificationEmployeeNotes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "NotificationDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NotificationEmployeeNoteId = table.Column<int>(type: "int", nullable: true),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    ReviewId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationDetails_NotificationEmployeeNotes_NotificationEmployeeNoteId",
                        column: x => x.NotificationEmployeeNoteId,
                        principalTable: "NotificationEmployeeNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationDetails_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationDetails_ProductReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "ProductReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_CustomerId",
                table: "NotificationDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationEmployeeNoteId",
                table: "NotificationDetails",
                column: "NotificationEmployeeNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationId",
                table: "NotificationDetails",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_ReviewId",
                table: "NotificationDetails",
                column: "ReviewId");
        }
    }
}
