using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenamedImageIdFieldInCustomersTableBackToImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Media_ImageId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ImageId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ImageId",
                table: "Customers",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Media_ImageId",
                table: "Customers",
                column: "ImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
