
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Infrastructure.Common.Persistence.Data;

namespace StrikeDefender.Infrastructure.AttackResults.Persistance;

public class AttackResultRepository(StrikeDefenderDbContext dbContext) : IGenericRepository<AttackResult>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(AttackResult entity, CancellationToken ct = default)
        => await _db.AttackResults.AddAsync(entity, ct);

    public Task UpdateAsync(AttackResult entity, CancellationToken ct = default)
    {
        _db.AttackResults.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AttackResult entity, CancellationToken ct = default)
    {
        _db.AttackResults.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<AttackResult?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AttackResults
            .Include(x => x.Attack)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.AttackResults.AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<AttackResult>> ListAsync(RequestFilters filters, CancellationToken ct = default)
    {
        var query = _db.AttackResults
            .Include(x => x.Attack)
            .Where(x => !x.Deleted)
            .AsQueryable();

        var total = await query.CountAsync(ct);

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy(filters.SortColumn + " " + filters.SortDirection);
        else
            query = query.OrderByDescending(x => x.Createdon);

        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query.AsNoTracking().ToListAsync(ct);

        return new PaginatedList<AttackResult>(items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<AttackResult>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<AttackResult>();

        var s = keyword.ToLower();

        return await _db.AttackResults
            .Include(x => x.Attack)
            .Where(x => !x.Deleted &&
                       x.Attack.Payload.ToLower().Contains(s))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
