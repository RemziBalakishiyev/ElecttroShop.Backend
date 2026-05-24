using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionalFieldsToBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPromotional",
                table: "Brands",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Brands",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_IsPromotional_DisplayOrder",
                table: "Brands",
                columns: new[] { "IsPromotional", "DisplayOrder" },
                filter: "\"IsPromotional\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brands_IsPromotional_DisplayOrder",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "IsPromotional",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Brands");
        }
    }
}






