using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_BrandId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_CategoryId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_ProductId",
                table: "Discounts");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanner",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsBanner",
                table: "Products",
                column: "IsBanner",
                filter: "\"IsBanner\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsFeatured_DisplayOrder",
                table: "Products",
                columns: new[] { "IsFeatured", "DisplayOrder" },
                filter: "\"IsFeatured\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_BrandId",
                table: "Discounts",
                columns: new[] { "Type", "BrandId" },
                filter: "\"BrandId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_CategoryId",
                table: "Discounts",
                columns: new[] { "Type", "CategoryId" },
                filter: "\"CategoryId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_ProductId",
                table: "Discounts",
                columns: new[] { "Type", "ProductId" },
                filter: "\"ProductId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_IsBanner",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsFeatured_DisplayOrder",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_BrandId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_CategoryId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Type_ProductId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsBanner",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_BrandId",
                table: "Discounts",
                columns: new[] { "Type", "BrandId" },
                filter: "[BrandId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_CategoryId",
                table: "Discounts",
                columns: new[] { "Type", "CategoryId" },
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Type_ProductId",
                table: "Discounts",
                columns: new[] { "Type", "ProductId" },
                filter: "[ProductId] IS NOT NULL");
        }
    }
}
