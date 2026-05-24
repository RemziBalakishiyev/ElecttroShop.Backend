using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAttributeUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedValue",
                table: "CategoryAttributeValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                computedColumnSql: "TRIM(\"Value\")",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedAttributeType",
                table: "CategoryAttributes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                computedColumnSql: "LOWER(TRIM(\"AttributeType\"))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues",
                columns: new[] { "CategoryAttributeId", "NormalizedValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_CategoryAttributes_CategoryId_NormalizedAttributeType",
                table: "CategoryAttributes",
                columns: new[] { "CategoryId", "NormalizedAttributeType" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.DropIndex(
                name: "UX_CategoryAttributes_CategoryId_NormalizedAttributeType",
                table: "CategoryAttributes");

            migrationBuilder.DropColumn(
                name: "NormalizedValue",
                table: "CategoryAttributeValues");

            migrationBuilder.DropColumn(
                name: "NormalizedAttributeType",
                table: "CategoryAttributes");
        }
    }
}