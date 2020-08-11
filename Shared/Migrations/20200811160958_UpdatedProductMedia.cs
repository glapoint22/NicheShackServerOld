using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedProductMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMedia",
                table: "ProductMedia");

            migrationBuilder.AlterColumn<int>(
                name: "MediaId",
                table: "ProductMedia",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductMedia",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMedia",
                table: "ProductMedia",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductMedia",
                table: "ProductMedia");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_ProductId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductMedia");

            migrationBuilder.AlterColumn<int>(
                name: "MediaId",
                table: "ProductMedia",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductMedia",
                table: "ProductMedia",
                columns: new[] { "ProductId", "MediaId" });
        }
    }
}
