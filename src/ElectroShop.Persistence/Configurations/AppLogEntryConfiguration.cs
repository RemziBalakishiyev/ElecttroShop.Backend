using ElectroShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectroShop.Persistence.Configurations;

public class AppLogEntryConfiguration : IEntityTypeConfiguration<AppLogEntry>
{
    public void Configure(EntityTypeBuilder<AppLogEntry> builder)
    {
        builder.ToTable("AppLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Level)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Message)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.Exception)
            .HasColumnType("text");

        builder.Property(e => e.SourceContext)
            .HasMaxLength(500);

        builder.Property(e => e.EventType)
            .HasMaxLength(100);

        builder.Property(e => e.CorrelationId)
            .HasMaxLength(64);

        builder.Property(e => e.UserEmail)
            .HasMaxLength(256);

        builder.Property(e => e.RequestPath)
            .HasMaxLength(2048);

        builder.Property(e => e.RequestMethod)
            .HasMaxLength(16);

        builder.Property(e => e.QueryString)
            .HasMaxLength(4096);

        builder.Property(e => e.RequestBody)
            .HasColumnType("text");

        builder.Property(e => e.ClientIp)
            .HasMaxLength(64);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(1024);

        builder.Property(e => e.MachineName)
            .HasMaxLength(256);

        builder.Property(e => e.PropertiesJson)
            .HasColumnType("jsonb");

        builder.HasIndex(e => e.TimestampUtc);
        builder.HasIndex(e => e.Level);
        builder.HasIndex(e => e.CorrelationId);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.TimestampUtc, e.Level });
    }
}
