using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingBrandPromotionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Orphaned migration: 20251130200000_AddPromotionalFieldsToBrand
            migrationBuilder.Sql("""
                ALTER TABLE "Brands" ADD COLUMN IF NOT EXISTS "IsPromotional" boolean NOT NULL DEFAULT false;
                ALTER TABLE "Brands" ADD COLUMN IF NOT EXISTS "DisplayOrder" integer NULL;
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_Brands_IsPromotional_DisplayOrder"
                ON "Brands" ("IsPromotional", "DisplayOrder")
                WHERE "IsPromotional" = true;
                """);

            // Orphaned migration: 20251130193207_AddBannerToProduct2
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_Products_IsBanner";
                DROP INDEX IF EXISTS "IX_Products_IsFeatured_DisplayOrder";
                """);

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

            // Orphaned migration: 20251216212428_RemoveSkuPriceStockFromProductVariant
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_ProductVariants_Sku";
                ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Sku";
                ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Price";
                ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Currency";
                ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Stock";
                """);

            // Orphaned migration: 20260523120000_FixRowVersionUsePostgresXmin
            // PostgreSQL xmin is a system column; only remove the legacy bytea RowVersion columns.
            migrationBuilder.Sql("""
                ALTER TABLE "Products" DROP COLUMN IF EXISTS "RowVersion";
                ALTER TABLE "Orders" DROP COLUMN IF EXISTS "RowVersion";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Orders",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "ProductVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "ProductVariants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "UNSET");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Products_IsFeatured_DisplayOrder",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsBanner",
                table: "Products");

            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_Brands_IsPromotional_DisplayOrder";
                ALTER TABLE "Brands" DROP COLUMN IF EXISTS "DisplayOrder";
                ALTER TABLE "Brands" DROP COLUMN IF EXISTS "IsPromotional";
                """);
        }
    }
}
