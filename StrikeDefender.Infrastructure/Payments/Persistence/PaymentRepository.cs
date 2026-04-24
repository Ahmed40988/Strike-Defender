using ErrorOr;
using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Payments;
using StrikeDefender.Infrastructure.Common.Persistence.Data;

namespace StrikeDefender.Infrastructure.Payments.Persistence;

public class PaymentRepository(StrikeDefenderDbContext dbContext)
    : IGenericRepository<PaymentTransaction>, IPaymentTransactionRepository
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(PaymentTransaction entity, CancellationToken ct = default)
        => await _db.Set<PaymentTransaction>().AddAsync(entity, ct);

    public Task UpdateAsync(PaymentTransaction entity, CancellationToken ct = default)
    {
        _db.Set<PaymentTransaction>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PaymentTransaction entity, CancellationToken ct = default)
    {
        _db.Set<PaymentTransaction>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<PaymentTransaction>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<PaymentTransaction>()
            .AnyAsync(x => x.Id == id, ct);

    // 🔥 أهم فانكشن عندك
    public async Task<PaymentTransaction?> GetByOrderIdAsync(int orderId, CancellationToken ct = default)
        => await _db.Set<PaymentTransaction>()
            .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);

    // 🔥 لو عايز تجيب payments ليوزر معين
    public async Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await _db.Set<PaymentTransaction>()
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync(ct);

    // 🔥 Pagination (اختياري زي plans)
    public async Task<PaginatedList<PaymentTransaction>> ListAsync(
        RequestFilters filters,
        CancellationToken ct = default)
    {
        var query = _db.Set<PaymentTransaction>().AsQueryable();

        var total = await query.CountAsync(ct);

        query = query.OrderByDescending(x => x.Createdon);

        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query.AsNoTracking().ToListAsync(ct);

        return new PaginatedList<PaymentTransaction>(
            items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task AddRangeAsync(IEnumerable<PaymentTransaction> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public IQueryable<PaymentTransaction> Query()
    {
        return _db.Set<PaymentTransaction>().AsQueryable();
    }

    public Task<IReadOnlyList<PaymentTransaction>> SearchAsync(string keyword, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}