using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ChangedImageFieldInCustomersTableToImageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Id",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ImageId",
                table: "Customers",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Customers",
                column: "NormalizedEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Media_ImageId",
                table: "Customers",
                column: "ImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Media_ImageId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ImageId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Id",
                table: "Customers",
                column: "Id")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "FirstName", "Image" });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Customers",
                column: "NormalizedEmail")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "UserName", "NormalizedUserName", "Email", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FirstName", "LastName", "ReviewName", "Image", "EmailPrefAddedListItem", "EmailPrefDeletedList", "EmailPrefEmailChange", "EmailPrefListNameChange", "EmailPrefMovedListItem", "EmailPrefNameChange", "EmailPrefNewCollaborator", "EmailPrefPasswordChange", "EmailPrefProfilePicChange", "EmailPrefRemovedCollaborator", "EmailPrefRemovedListItem", "EmailPrefReview" });
        }
    }
}
