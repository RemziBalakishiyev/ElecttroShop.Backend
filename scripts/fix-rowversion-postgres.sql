-- PostgreSQL: SQL Server rowversion (bytea) sütunlarını sil.
-- EF Core RowVersion artıq PostgreSQL xmin system column-una map olunur.

ALTER TABLE "Products" DROP COLUMN IF EXISTS "RowVersion";
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "RowVersion";
