using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedPricePointProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAdditionalInfo");

            migrationBuilder.DropTable(
                name: "ProductPriceAdditionalInfo");

            migrationBuilder.DropTable(
                name: "ProductPrices");

            migrationBuilder.AddColumn<int>(
                name: "RebillFrequency",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RecurringPrice",
                table: "Products",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingType",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionDuration",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeFrameBetweenRebill",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrialPeriod",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Customers",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "PricePoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    ImageId = table.Column<int>(nullable: true),
                    Header = table.Column<string>(maxLength: 50, nullable: true),
                    Quantity = table.Column<string>(maxLength: 50, nullable: true),
                    UnitPrice = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(maxLength: 25, nullable: true),
                    StrikethroughPrice = table.Column<string>(nullable: true),
                    Price = table.Column<string>(nullable: true),
                    ShippingType = table.Column<int>(nullable: false),
                    TrialPeriod = table.Column<int>(nullable: false),
                    RecurringPrice = table.Column<double>(nullable: false),
                    RebillFrequency = table.Column<int>(nullable: false),
                    TimeFrameBetweenRebill = table.Column<int>(nullable: false),
                    SubscriptionDuration = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricePoints_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PricePoints_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricePoints_ImageId",
                table: "PricePoints",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PricePoints_ProductId",
                table: "PricePoints",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricePoints");

            migrationBuilder.DropColumn(
                name: "RebillFrequency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RecurringPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShippingType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubscriptionDuration",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TimeFrameBetweenRebill",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TrialPeriod",
                table: "Products");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "ProductAdditionalInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    RebillFrequency = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShippingType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SubscriptionDuration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TimeFrameBetweenRebill = table.Column<int>(type: "int", nullable: true),
                    TrialPeriod = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdditionalInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAdditionalInfo_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrices",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Header = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    Quantity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StrikethroughPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    UnitPrice = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ProductPriceAdditionalInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Price = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductPriceId = table.Column<int>(type: "int", nullable: false),
                    RebillFrequency = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ShippingType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SubscriptionDuration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TimeFrameBetweenRebill = table.Column<int>(type: "int", nullable: true),
                    TrialPeriod = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceAdditionalInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPriceAdditionalInfo_ProductPrices_ProductId_ProductPriceId",
                        columns: x => new { x.ProductId, x.ProductPriceId },
                        principalTable: "ProductPrices",
                        principalColumns: new[] { "ProductId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdditionalInfo_ProductId",
                table: "ProductAdditionalInfo",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "IsRecurring", "ShippingType", "TrialPeriod", "Price", "RebillFrequency", "TimeFrameBetweenRebill", "SubscriptionDuration" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceAdditionalInfo_ProductPriceId",
                table: "ProductPriceAdditionalInfo",
                column: "ProductPriceId")
                .Annotation("SqlServer:Clustered", false)
                .Annotation("SqlServer:Include", new[] { "Id", "IsRecurring", "ShippingType", "TrialPeriod", "Price", "RebillFrequency", "TimeFrameBetweenRebill", "SubscriptionDuration" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceAdditionalInfo_ProductId_ProductPriceId",
                table: "ProductPriceAdditionalInfo",
                columns: new[] { "ProductId", "ProductPriceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_ImageId",
                table: "ProductPrices",
                column: "ImageId");
        }
    }
}
