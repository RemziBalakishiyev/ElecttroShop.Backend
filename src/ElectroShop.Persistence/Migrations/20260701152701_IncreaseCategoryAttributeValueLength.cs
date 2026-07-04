using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseCategoryAttributeValueLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.DropColumn(
                name: "NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CategoryAttributeValues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedValue",
                table: "CategoryAttributeValues",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                computedColumnSql: "TRIM(\"Value\")",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues",
                columns: new[] { "CategoryAttributeId", "NormalizedValue" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.DropColumn(
                name: "NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "CategoryAttributeValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedValue",
                table: "CategoryAttributeValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                computedColumnSql: "TRIM(\"Value\")",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues",
                columns: new[] { "CategoryAttributeId", "NormalizedValue" },
                unique: true);
        }
    }
}
