using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddReviewIdToNotificationText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "NotificationText",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationText_ReviewId",
                table: "NotificationText",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationText_ProductReviews_ReviewId",
                table: "NotificationText",
                column: "ReviewId",
                principalTable: "ProductReviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationText_ProductReviews_ReviewId",
                table: "NotificationText");

            migrationBuilder.DropIndex(
                name: "IX_NotificationText_ReviewId",
                table: "NotificationText");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "NotificationText");
        }
    }
}
