using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToAggregateRoots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Yalnız RowVersion sütunlarını əlavə et
            // Index və sütun silmə əməliyyatları artıq əvvəlki migration-da edilib
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Yalnız RowVersion sütunlarını sil
            // Digər dəyişikliklər əvvəlki migration-da edilib
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Orders");
        }
    }
}
