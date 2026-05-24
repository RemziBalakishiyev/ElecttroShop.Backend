using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations;

public partial class FixRowVersionUsePostgresXmin : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "RowVersion",
            table: "Orders");

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Products",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(
            name: "xmin",
            table: "Orders",
            type: "xid",
            rowVersion: true,
            nullable: false,
            defaultValue: 0u);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "xmin",
            table: "Orders");

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