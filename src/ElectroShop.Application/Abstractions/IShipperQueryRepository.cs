using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Abstractions;

/// <summary>
/// Shipper-specific query repository
/// </summary>
public interface IShipperQueryRepository : IQueryRepository<Shipper>
{
    /// <summary>
    /// E-poçt ünvanına görə shipper tapır
    /// </summary>
    Task<Shipper?> GetShipperByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forwarding Freight ID-yə görə shipperləri tapır
    /// </summary>
    Task<IEnumerable<Shipper>> GetShippersByForwardingFreightIdAsync(Guid forwardingFreightId, CancellationToken cancellationToken = default);
}

