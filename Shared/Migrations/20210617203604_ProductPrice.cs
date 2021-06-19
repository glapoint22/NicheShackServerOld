using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ProductPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceIndices");

            migrationBuilder.DropTable(
                name: "ProductPricePoints");

            migrationBuilder.DropTable(
                name: "ProductContent");

            migrationBuilder.DropIndex(
                name: "IX_Products_UrlId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name_Id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiPrice",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<int>(nullable: true),
                    Header = table.Column<string>(maxLength: 50, nullable: true),
                    Quantity = table.Column<string>(maxLength: 50, nullable: true),
                    UnitPrice = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(maxLength: 25, nullable: true),
                    StrikethroughPrice = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false, defaultValue: 0.0),
                    Shipping = table.Column<int>(nullable: false, defaultValue: 0),
                    ShippingPrice = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrices", x => new { x.ProductId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProductPrices_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPrices_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UrlId",
                table: "Products",
                column: "UrlId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "ImageId", "Name", "Hoplink", "Description", "TotalReviews", "Rating", "OneStar", "TwoStars", "ThreeStars", "FourStars", "FiveStars", "UrlName" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_Id",
                table: "Products",
                columns: new[] { "Name", "Id" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ImageId", "NicheId", "UrlId", "UrlName", "TotalReviews", "Rating", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ImageId",
                table: "ProductPrices",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.DropIndex(
                name: "IX_Products_UrlId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Name_Id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsMultiPrice",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "MaxPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ProductContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IconId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductContent_Media_IconId",
                        column: x => x.IconId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductContent_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPricePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Decimal = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TextAfter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextBefore = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WholeNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPricePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPricePoints_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceIndices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Index = table.Column<int>(type: "int", nullable: false),
                    ProductContentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceIndices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceIndices_ProductContent_ProductContentId",
                        column: x => x.ProductContentId,
                        principalTable: "ProductContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UrlId",
                table: "Products",
                column: "UrlId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "ImageId", "Name", "Hoplink", "Description", "MinPrice", "MaxPrice", "TotalReviews", "Rating", "OneStar", "TwoStars", "ThreeStars", "FourStars", "FiveStars", "UrlName" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name_Id",
                table: "Products",
                columns: new[] { "Name", "Id" })
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "ImageId", "NicheId", "UrlId", "UrlName", "MinPrice", "MaxPrice", "TotalReviews", "Rating", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceIndices_ProductContentId",
                table: "PriceIndices",
                column: "ProductContentId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_IconId",
                table: "ProductContent",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductContent_ProductId",
                table: "ProductContent",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "IconId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPricePoints_ProductId",
                table: "ProductPricePoints",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "TextBefore", "WholeNumber", "Decimal", "TextAfter", "Index" });
        }
    }
}
