using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedNiches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Niches",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Niches_ImageId",
                table: "Niches",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Niches_Media_ImageId",
                table: "Niches",
                column: "ImageId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Niches_Media_ImageId",
                table: "Niches");

            migrationBuilder.DropIndex(
                name: "IX_Niches_ImageId",
                table: "Niches");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Niches");
        }
    }
}
