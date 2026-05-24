using ElectroShop.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ElectroShop.Persistence.Exceptions;

/// <summary>
/// Maps PostgreSQL unique constraint violations to domain-friendly errors.
/// </summary>
public static class DatabaseExceptionMapper
{
    public const string CategoryAttributeUniqueIndex =
        "UX_CategoryAttributes_CategoryId_NormalizedAttributeType";

    public const string CategoryAttributeValueUniqueIndex =
        "UX_CategoryAttributeValues_CategoryAttributeId_NormalizedValue";

    public static Error? TryMap(DbUpdateException exception)
    {
        var postgresException = FindPostgresException(exception);
        if (postgresException is null)
            return null;

        if (postgresException.SqlState != PostgresErrorCodes.UniqueViolation)
            return null;

        if (string.Equals(
                postgresException.ConstraintName,
                CategoryAttributeUniqueIndex,
                StringComparison.OrdinalIgnoreCase))
        {
            return DomainErrors.ProductVariant.AttributeDuplicateConstraint;
        }

        if (string.Equals(
                postgresException.ConstraintName,
                CategoryAttributeValueUniqueIndex,
                StringComparison.OrdinalIgnoreCase))
        {
            return DomainErrors.ProductVariant.ValueDuplicateConstraint;
        }

        return null;
    }

    private static PostgresException? FindPostgresException(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgresException)
                return postgresException;
        }

        return null;
    }
}