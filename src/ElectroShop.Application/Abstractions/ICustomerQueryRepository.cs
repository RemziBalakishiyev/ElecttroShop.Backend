using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Customer-specific query repository
/// </summary>
public interface ICustomerQueryRepository : IQueryRepository<Customer>
{
    /// <summary>
    /// E-poçt ünvanına görə müştəri tapır
    /// </summary>
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
}

