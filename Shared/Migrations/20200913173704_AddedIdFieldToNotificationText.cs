using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddedIdFieldToNotificationText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationText",
                table: "NotificationText");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationText",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NotificationText",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "NotificationText",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "NotificationText",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationText",
                table: "NotificationText",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationText_CustomerId",
                table: "NotificationText",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationText",
                table: "NotificationText");

            migrationBuilder.DropIndex(
                name: "IX_NotificationText_CustomerId",
                table: "NotificationText");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "NotificationText");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NotificationText",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NotificationText",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "NotificationText",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationText",
                table: "NotificationText",
                columns: new[] { "CustomerId", "NotificationId" });
        }
    }
}
