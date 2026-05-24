using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ElectroShop.Persistence.Repositories;

public class ShipperQueryRepository : QueryRepository<Shipper>, IShipperQueryRepository
{
    public ShipperQueryRepository(ElectroShopDbContext context)
        : base(context)
    {
    }

    public async Task<Shipper?> GetShipperByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailLower = email.ToLowerInvariant();
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Email == emailLower && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Shipper>> GetShippersByForwardingFreightIdAsync(Guid forwardingFreightId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.ForwardingFreightId == forwardingFreightId && !s.IsDeleted)
            .OrderBy(s => s.FullName)
            .ToListAsync(cancellationToken);
    }
}

