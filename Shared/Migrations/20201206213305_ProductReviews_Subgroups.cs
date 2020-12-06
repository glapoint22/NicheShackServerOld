using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class ProductReviews_Subgroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubgroupProducts",
                table: "SubgroupProducts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SubgroupProducts",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProductReviews",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubgroupProducts",
                table: "SubgroupProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubgroupProducts_ProductId",
                table: "SubgroupProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubgroupProducts",
                table: "SubgroupProducts");

            migrationBuilder.DropIndex(
                name: "IX_SubgroupProducts_ProductId",
                table: "SubgroupProducts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubgroupProducts");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProductReviews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubgroupProducts",
                table: "SubgroupProducts",
                columns: new[] { "ProductId", "SubgroupId" });
        }
    }
}
