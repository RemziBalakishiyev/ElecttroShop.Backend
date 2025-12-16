using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSkuPriceStockFromProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop index first
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants");

            // Drop Sku column (owned entity)
            migrationBuilder.DropColumn(
                name: "Sku",
                table: "ProductVariants");

            // Drop Price owned entity columns
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "ProductVariants");

            // Drop Stock column
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "ProductVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add Stock column back
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Add Price owned entity columns back
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVariants",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "ProductVariants",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "AZN");

            // Add Sku owned entity column back
            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "UNSET");

            // Recreate index
            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);
        }
    }
}

