using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Repositories;

public class CustomerQueryRepository : QueryRepository<Customer>, ICustomerQueryRepository
{
    public CustomerQueryRepository(ElectroShopDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailLower = email.ToLowerInvariant();
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == emailLower && !c.IsDeleted, cancellationToken);
    }
}

