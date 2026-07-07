using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations;

[DbContext(typeof(ElectroShopDbContext))]
[Migration("20260706040000_AddCloudinaryFieldsToProductImages")]
/// <inheritdoc />
public partial class AddCloudinaryFieldsToProductImages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "ImageUrl" character varying(2048);
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "PublicId" character varying(512);
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "ImagePath" character varying(1024);
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "FileName" character varying(512);
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "ContentType" character varying(128);
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "Size" bigint;
            ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "StorageProvider" character varying(64) NOT NULL DEFAULT 'Cloudinary';
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "ImageUrl";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "PublicId";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "ImagePath";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "FileName";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "ContentType";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "Size";
            ALTER TABLE "ProductImages" DROP COLUMN IF EXISTS "StorageProvider";
            """);
    }
}
