using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
	public partial class InitialMigration : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Products",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
					ImgUri = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Products");
		}
	}
}
