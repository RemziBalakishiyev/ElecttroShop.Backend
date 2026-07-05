using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ElectroShop.Persistence.Contexts;

#nullable disable

namespace ElectroShop.Persistence.Migrations;

[DbContext(typeof(ElectroShopDbContext))]
[Migration("20260706020000_EnsureProductAttributesTables")]
/// <inheritdoc />
public partial class EnsureProductAttributesTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "ProductAttributes" (
                "Id" uuid NOT NULL,
                "ProductId" uuid NOT NULL,
                "Name" character varying(100) NOT NULL,
                "DisplayName" character varying(200) NOT NULL,
                "AttributeType" character varying(100) NOT NULL,
                "IsRequired" boolean NOT NULL DEFAULT FALSE,
                "DisplayOrder" integer NOT NULL DEFAULT 0,
                "CreatedAtUtc" timestamp with time zone NOT NULL,
                "CreatedBy" character varying(200),
                "IsDeleted" boolean NOT NULL DEFAULT FALSE,
                "UpdatedAtUtc" timestamp with time zone,
                "UpdatedBy" character varying(200),
                CONSTRAINT "PK_ProductAttributes" PRIMARY KEY ("Id"),
                CONSTRAINT "FK_ProductAttributes_Products_ProductId" FOREIGN KEY ("ProductId")
                    REFERENCES "Products" ("Id") ON DELETE CASCADE
            );
            """);

        migrationBuilder.Sql("""
            CREATE INDEX IF NOT EXISTS "IX_ProductAttributes_IsDeleted" ON "ProductAttributes" ("IsDeleted");
            CREATE INDEX IF NOT EXISTS "IX_ProductAttributes_ProductId_DisplayOrder" ON "ProductAttributes" ("ProductId", "DisplayOrder");
            """);

        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "ProductAttributeValues" (
                "Id" uuid NOT NULL,
                "ProductAttributeId" uuid NOT NULL,
                "Value" character varying(500) NOT NULL,
                "DisplayValue" character varying(500),
                "ColorCode" character varying(7),
                "DisplayOrder" integer NOT NULL DEFAULT 0,
                CONSTRAINT "PK_ProductAttributeValues" PRIMARY KEY ("Id"),
                CONSTRAINT "FK_ProductAttributeValues_ProductAttributes_ProductAttributeId" FOREIGN KEY ("ProductAttributeId")
                    REFERENCES "ProductAttributes" ("Id") ON DELETE CASCADE
            );
            """);

        migrationBuilder.Sql("""
            CREATE INDEX IF NOT EXISTS "IX_ProductAttributeValues_ProductAttributeId_DisplayOrder"
                ON "ProductAttributeValues" ("ProductAttributeId", "DisplayOrder");
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProductAttributeValues");
        migrationBuilder.DropTable(name: "ProductAttributes");
    }
}
