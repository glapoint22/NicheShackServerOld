using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class Keywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeywordGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    ForProduct = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeywordGroups_Belonging_To_Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    KeywordGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordGroups_Belonging_To_Product", x => new { x.ProductId, x.KeywordGroupId });
                    table.ForeignKey(
                        name: "FK_KeywordGroups_Belonging_To_Product_KeywordGroups_KeywordGroupId",
                        column: x => x.KeywordGroupId,
                        principalTable: "KeywordGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordGroups_Belonging_To_Product_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Keywords_In_KeywordGroup",
                columns: table => new
                {
                    KeywordGroupId = table.Column<int>(nullable: false),
                    KeywordId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords_In_KeywordGroup", x => new { x.KeywordGroupId, x.KeywordId });
                    table.ForeignKey(
                        name: "FK_Keywords_In_KeywordGroup_KeywordGroups_KeywordGroupId",
                        column: x => x.KeywordGroupId,
                        principalTable: "KeywordGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Keywords_In_KeywordGroup_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeywordGroups_Belonging_To_Product_KeywordGroupId",
                table: "KeywordGroups_Belonging_To_Product",
                column: "KeywordGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_In_KeywordGroup_KeywordId",
                table: "Keywords_In_KeywordGroup",
                column: "KeywordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeywordGroups_Belonging_To_Product");

            migrationBuilder.DropTable(
                name: "Keywords_In_KeywordGroup");

            migrationBuilder.DropTable(
                name: "KeywordGroups");
        }
    }
}
