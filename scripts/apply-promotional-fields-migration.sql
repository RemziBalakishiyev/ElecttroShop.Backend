-- Migration: AddPromotionalFieldsToBrand
-- Bu script-i PostgreSQL database-də işlədin

-- 1. IsPromotional sütununu əlavə et
ALTER TABLE "Brands" 
ADD COLUMN "IsPromotional" boolean NOT NULL DEFAULT false;

-- 2. DisplayOrder sütununu əlavə et
ALTER TABLE "Brands" 
ADD COLUMN "DisplayOrder" integer NULL;

-- 3. Index yarat
CREATE INDEX "IX_Brands_IsPromotional_DisplayOrder" 
ON "Brands" ("IsPromotional", "DisplayOrder") 
WHERE "IsPromotional" = true;

-- 4. Migration history-yə əlavə et (əgər __EFMigrationsHistory cədvəli varsa)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251130200000_AddPromotionalFieldsToBrand', '8.0.21')
ON CONFLICT ("MigrationId") DO NOTHING;

