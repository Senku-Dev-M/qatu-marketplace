using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qatu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "qatu",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Presentation",
                schema: "qatu",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                schema: "qatu",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "unidad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "qatu",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Presentation",
                schema: "qatu",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "qatu",
                table: "Products");
        }
    }
}
