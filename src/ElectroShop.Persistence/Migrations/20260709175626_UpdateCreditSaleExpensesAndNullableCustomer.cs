using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCreditSaleExpensesAndNullableCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expenses",
                table: "CreditSales",
                newName: "TotalExpenses");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "CreditSales",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "CreditSales",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateTable(
                name: "CreditSaleExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditSaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpenseType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditSaleExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditSaleExpenses_CreditSales_CreditSaleId",
                        column: x => x.CreditSaleId,
                        principalTable: "CreditSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditSaleExpenses_CreditSaleId",
                table: "CreditSaleExpenses",
                column: "CreditSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSaleExpenses_ExpenseType",
                table: "CreditSaleExpenses",
                column: "ExpenseType");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSaleExpenses_IsDeleted",
                table: "CreditSaleExpenses",
                column: "IsDeleted");

            migrationBuilder.Sql("""
                INSERT INTO "CreditSaleExpenses" (
                    "Id", "CreditSaleId", "ExpenseType", "Description", "Amount",
                    "CreatedAtUtc", "IsDeleted")
                SELECT gen_random_uuid(), "Id", 'Other', 'Köhnə xərc', "TotalExpenses",
                    NOW() AT TIME ZONE 'UTC', false
                FROM "CreditSales"
                WHERE "TotalExpenses" > 0 AND NOT "IsDeleted";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditSaleExpenses");

            migrationBuilder.RenameColumn(
                name: "TotalExpenses",
                table: "CreditSales",
                newName: "Expenses");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "CreditSales",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "CreditSales",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
