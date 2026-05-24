BEGIN;

DROP INDEX IF EXISTS "IX_Products_IsBanner";
DROP INDEX IF EXISTS "IX_Products_IsFeatured_DisplayOrder";

CREATE INDEX "IX_Products_IsBanner"
    ON "Products" ("IsBanner")
    WHERE "IsBanner" = true;

CREATE INDEX "IX_Products_IsFeatured_DisplayOrder"
    ON "Products" ("IsFeatured", "DisplayOrder")
    WHERE "IsFeatured" = true;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251130193207_AddBannerToProduct2', '8.0.21')
ON CONFLICT ("MigrationId") DO NOTHING;

ALTER TABLE "Brands"
    ADD COLUMN IF NOT EXISTS "IsPromotional" boolean NOT NULL DEFAULT false;

ALTER TABLE "Brands"
    ADD COLUMN IF NOT EXISTS "DisplayOrder" integer NULL;

CREATE INDEX IF NOT EXISTS "IX_Brands_IsPromotional_DisplayOrder"
    ON "Brands" ("IsPromotional", "DisplayOrder")
    WHERE "IsPromotional" = true;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251130200000_AddPromotionalFieldsToBrand', '8.0.21')
ON CONFLICT ("MigrationId") DO NOTHING;

DROP INDEX IF EXISTS "IX_ProductVariants_Sku";

ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Sku";
ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Price";
ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Currency";
ALTER TABLE "ProductVariants" DROP COLUMN IF EXISTS "Stock";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251216212428_RemoveSkuPriceStockFromProductVariant', '8.0.21')
ON CONFLICT ("MigrationId") DO NOTHING;

COMMIT;