using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Plans.Persistance;

public class PlanRepository(StrikeDefenderDbContext dbContext)
    : IGenericRepository<Plan>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(Plan entity, CancellationToken ct = default)
        => await _db.Planes.AddAsync(entity, ct);

    public Task UpdateAsync(Plan entity, CancellationToken ct = default)
    {
        _db.Planes.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Plan entity, CancellationToken ct = default)
    {
        _db.Planes.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Planes
            .Include(x => x.Subscriptions)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.Planes
            .AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<Plan>> ListAsync(
        RequestFilters filters,
        CancellationToken ct = default)
    {
        var query = _db.Planes
            .Where(x => !x.Deleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy(filters.SortColumn + " " + filters.SortDirection);
        else
            query = query.OrderBy(x => x.Price);

        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query.AsNoTracking().ToListAsync(ct);

        return new PaginatedList<Plan>(
            items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<Plan>> SearchAsync(
        string keyword,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<Plan>();

        var s = keyword.ToLower();

        return await _db.Planes
            .Where(x => !x.Deleted && x.Name.ToLower().Contains(s))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}