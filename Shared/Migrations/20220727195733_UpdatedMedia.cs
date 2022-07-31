using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class UpdatedMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "ImageAnySize",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageAnySizeHeight",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageAnySizeWidth",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageLg",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageLgHeight",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageLgWidth",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageMd",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageMdHeight",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageMdWidth",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageSm",
                table: "Media",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageSmHeight",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageSmWidth",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailHeight",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailWidth",
                table: "Media",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ImageReferences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<int>(nullable: false),
                    ImageSize = table.Column<int>(nullable: false),
                    Builder = table.Column<int>(nullable: false),
                    Host = table.Column<string>(maxLength: 256, nullable: false),
                    Location = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageReferences_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageReferences_ImageId",
                table: "ImageReferences",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageReferences");

            migrationBuilder.DropColumn(
                name: "ImageAnySize",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageAnySizeHeight",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageAnySizeWidth",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageLg",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageLgHeight",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageLgWidth",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageMd",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageMdHeight",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageMdWidth",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageSm",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageSmHeight",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ImageSmWidth",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ThumbnailHeight",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ThumbnailWidth",
                table: "Media");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Media",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
