﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
	public partial class PriceDecimal : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
				name: "Price",
				table: "Products",
				type: "decimal(18,2)",
				nullable: false,
				oldClrType: typeof(decimal),
				oldType: "decimal(18,4)");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<decimal>(
				name: "Price",
				table: "Products",
				type: "decimal(18,4)",
				nullable: false,
				oldClrType: typeof(decimal),
				oldType: "decimal(18,2)");
		}
	}
}
