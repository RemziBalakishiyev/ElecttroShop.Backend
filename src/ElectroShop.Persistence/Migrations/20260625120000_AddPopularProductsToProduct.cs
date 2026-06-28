using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    [DbContext(typeof(ElectroShopDbContext))]
    [Migration("20260625120000_AddPopularProductsToProduct")]
    /// <inheritdoc />
    public partial class AddPopularProductsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "IsPopular" boolean NOT NULL DEFAULT false;
                ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "PopularDisplayOrder" integer NULL;
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_Products_IsPopular_PopularDisplayOrder"
                ON "Products" ("IsPopular", "PopularDisplayOrder")
                WHERE "IsPopular" = true;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS "IX_Products_IsPopular_PopularDisplayOrder";
                ALTER TABLE "Products" DROP COLUMN IF EXISTS "PopularDisplayOrder";
                ALTER TABLE "Products" DROP COLUMN IF EXISTS "IsPopular";
                """);
        }
    }
}
