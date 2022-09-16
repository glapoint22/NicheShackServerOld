using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class TempVendors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempVendors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    WebPage = table.Column<string>(maxLength: 256, nullable: true),
                    Street = table.Column<string>(maxLength: 256, nullable: true),
                    City = table.Column<string>(maxLength: 256, nullable: true),
                    Zip = table.Column<int>(nullable: true),
                    PoBox = table.Column<int>(nullable: true),
                    State = table.Column<string>(maxLength: 2, nullable: true),
                    Country = table.Column<string>(maxLength: 256, nullable: true),
                    PrimaryFirstName = table.Column<string>(maxLength: 256, nullable: true),
                    PrimaryLastName = table.Column<string>(maxLength: 256, nullable: true),
                    PrimaryOfficePhone = table.Column<string>(maxLength: 20, nullable: true),
                    PrimaryMobilePhone = table.Column<string>(maxLength: 20, nullable: true),
                    PrimaryEmail = table.Column<string>(maxLength: 256, nullable: true),
                    SecondaryFirstName = table.Column<string>(maxLength: 256, nullable: true),
                    SecondaryLastName = table.Column<string>(maxLength: 256, nullable: true),
                    SecondaryOfficePhone = table.Column<string>(maxLength: 20, nullable: true),
                    SecondaryMobilePhone = table.Column<string>(maxLength: 20, nullable: true),
                    SecondaryEmail = table.Column<string>(maxLength: 256, nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempVendors", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempVendors");
        }
    }
}
