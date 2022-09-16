using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedPricePoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePoints_Products_ProductId",
                table: "PricePoints");

            migrationBuilder.DropIndex(
                name: "IX_PricePoints_ProductId",
                table: "PricePoints");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PricePoints");

            migrationBuilder.AddColumn<int>(
                name: "ProductPriceId",
                table: "PricePoints",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PricePoints_ProductPriceId",
                table: "PricePoints",
                column: "ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePoints_ProductPrices_ProductPriceId",
                table: "PricePoints",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricePoints_ProductPrices_ProductPriceId",
                table: "PricePoints");

            migrationBuilder.DropIndex(
                name: "IX_PricePoints_ProductPriceId",
                table: "PricePoints");

            migrationBuilder.DropColumn(
                name: "ProductPriceId",
                table: "PricePoints");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PricePoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PricePoints_ProductId",
                table: "PricePoints",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricePoints_Products_ProductId",
                table: "PricePoints",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
