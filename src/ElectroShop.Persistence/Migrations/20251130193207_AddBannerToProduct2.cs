using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerToProduct2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_IsBanner",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsFeatured_DisplayOrder",
                table: "Products");

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
        }
    }
}
