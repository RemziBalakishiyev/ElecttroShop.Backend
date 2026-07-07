using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations;

/// <summary>
/// PostgreSQL-də xmin sistem sütunudur — DROP/ADD mümkün deyil (0A000).
/// Köhnə EF migration-ları fiziki xmin əlavə etməyə cəhd etsə də, PostgreSQL bunu rədd edir.
/// Concurrency düzəlişi yalnız <see cref="Configurations.AggregateRootConfiguration{T}"/>-dadır:
/// RowVersion + IsRowVersion() → Npgsql sistem xmin-ə map edir.
/// </summary>
[DbContext(typeof(ElectroShopDbContext))]
[Migration("20260707191500_DropPhysicalXminColumns")]
public partial class DropPhysicalXminColumns : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Schema dəyişikliyi tələb olunmur — xmin PostgreSQL sistem sütunudur.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Geri alınacaq schema dəyişikliyi yoxdur.
    }
}
