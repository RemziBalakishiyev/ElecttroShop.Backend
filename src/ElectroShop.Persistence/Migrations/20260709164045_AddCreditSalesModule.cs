using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditSalesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Sales",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceCreditSaleId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CategoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TotalCostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalSaleAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Expenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    GrossProfit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetProfit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DebtDurationDays = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ConvertedSaleId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConvertedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditSales_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CreditSales_Sales_ConvertedSaleId",
                        column: x => x.ConvertedSaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Origin",
                table: "Sales",
                column: "Origin");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SourceCreditSaleId",
                table: "Sales",
                column: "SourceCreditSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_ConvertedSaleId",
                table: "CreditSales",
                column: "ConvertedSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_CreditDate",
                table: "CreditSales",
                column: "CreditDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_CustomerPhone",
                table: "CreditSales",
                column: "CustomerPhone");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_DueDate",
                table: "CreditSales",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_IsDeleted",
                table: "CreditSales",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_IsDeleted_CreditDate",
                table: "CreditSales",
                columns: new[] { "IsDeleted", "CreditDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_ProductId",
                table: "CreditSales",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditSales_Status",
                table: "CreditSales",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditSales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Origin",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SourceCreditSaleId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SourceCreditSaleId",
                table: "Sales");
        }
    }
}
