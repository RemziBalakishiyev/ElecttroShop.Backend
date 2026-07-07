using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations;

/// <summary>
/// bytea RowVersion sütunlarını silir. PostgreSQL xmin sistem sütunudur — fiziki xmin əlavə edilmir.
/// </summary>
public partial class FixRowVersionUsePostgresXmin : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "Products" DROP COLUMN IF EXISTS "RowVersion";
            ALTER TABLE "Orders" DROP COLUMN IF EXISTS "RowVersion";
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Products",
            type: "bytea",
            nullable: false,
            defaultValue: new byte[0]);

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Orders",
            type: "bytea",
            nullable: false,
            defaultValue: new byte[0]);
    }
}
