using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddedSubgroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subgroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subgroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubgroupProducts",
                columns: table => new
                {
                    SubgroupId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubgroupProducts", x => new { x.ProductId, x.SubgroupId });
                    table.ForeignKey(
                        name: "FK_SubgroupProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubgroupProducts_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubgroupProducts_SubgroupId",
                table: "SubgroupProducts",
                column: "SubgroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubgroupProducts");

            migrationBuilder.DropTable(
                name: "Subgroups");
        }
    }
}
