using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ProductAttributes / ProductAttributeValues artıq DB-də ola bilər (köhnə migration).
            // Bu migration yalnız satış xərcləri üçün schema əlavə edir.
            migrationBuilder.Sql("""
                ALTER TABLE "Sales"
                ADD COLUMN IF NOT EXISTS "TotalExpenses" numeric(18,2) NOT NULL DEFAULT 0;
                """);

            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "SaleExpenses" (
                    "Id" uuid NOT NULL,
                    "SaleId" uuid NOT NULL,
                    "ExpenseType" character varying(50) NOT NULL,
                    "Description" character varying(1000),
                    "Amount" numeric(18,2) NOT NULL,
                    "CreatedAtUtc" timestamp with time zone NOT NULL,
                    "CreatedBy" character varying(200),
                    "IsDeleted" boolean NOT NULL DEFAULT FALSE,
                    "UpdatedAtUtc" timestamp with time zone,
                    "UpdatedBy" character varying(200),
                    CONSTRAINT "PK_SaleExpenses" PRIMARY KEY ("Id"),
                    CONSTRAINT "FK_SaleExpenses_Sales_SaleId" FOREIGN KEY ("SaleId")
                        REFERENCES "Sales" ("Id") ON DELETE CASCADE
                );
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_SaleExpenses_ExpenseType" ON "SaleExpenses" ("ExpenseType");
                CREATE INDEX IF NOT EXISTS "IX_SaleExpenses_IsDeleted" ON "SaleExpenses" ("IsDeleted");
                CREATE INDEX IF NOT EXISTS "IX_SaleExpenses_SaleId" ON "SaleExpenses" ("SaleId");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleExpenses");

            migrationBuilder.DropColumn(
                name: "TotalExpenses",
                table: "Sales");
        }
    }
}
