using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AdditionalInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "ProductPrices");

            migrationBuilder.CreateTable(
                name: "ProductAdditionalInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    IsRecurring = table.Column<bool>(nullable: false, defaultValue: false),
                    ShippingType = table.Column<int>(nullable: false, defaultValue: 0),
                    TrialPeriod = table.Column<int>(nullable: false, defaultValue: 0),
                    Price = table.Column<double>(nullable: false, defaultValue: 0.0),
                    RebillFrequency = table.Column<int>(nullable: false, defaultValue: 0),
                    TimeFrameBetweenRebill = table.Column<int>(nullable: true),
                    SubscriptionDuration = table.Column<int>(nullable: false, defaultValue: 0)
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
                name: "ProductPriceAdditionalInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    ProductPriceId = table.Column<int>(nullable: false),
                    IsRecurring = table.Column<bool>(nullable: false, defaultValue: false),
                    ShippingType = table.Column<int>(nullable: false, defaultValue: 0),
                    TrialPeriod = table.Column<int>(nullable: false, defaultValue: 0),
                    Price = table.Column<double>(nullable: false, defaultValue: 0.0),
                    RebillFrequency = table.Column<int>(nullable: false, defaultValue: 0),
                    TimeFrameBetweenRebill = table.Column<int>(nullable: true),
                    SubscriptionDuration = table.Column<int>(nullable: false, defaultValue: 0)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAdditionalInfo");

            migrationBuilder.DropTable(
                name: "ProductPriceAdditionalInfo");

            migrationBuilder.AddColumn<int>(
                name: "Shipping",
                table: "ProductPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ShippingPrice",
                table: "ProductPrices",
                type: "float",
                nullable: true);
        }
    }
}
