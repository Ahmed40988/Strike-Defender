using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Rules;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Rules.Persistance;

public class RuleRepository(StrikeDefenderDbContext dbContext) : IGenericRepository<WafRule>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(WafRule entity, CancellationToken ct = default)
        => await _db.wafRules.AddAsync(entity, ct);

    public Task UpdateAsync(WafRule entity, CancellationToken ct = default)
    {
        _db.wafRules.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(WafRule entity, CancellationToken ct = default)
    {
        _db.wafRules.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<WafRule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.wafRules
            .Include(x => x.Attacks)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.wafRules
            .AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<WafRule>> ListAsync(RequestFilters filters, CancellationToken ct = default)
    {
        var query = _db.wafRules
            .Where(x => !x.Deleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();
            query = query.Where(x =>
                x.RuleContent.ToLower().Contains(s) ||
                x.Description.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy(filters.SortColumn + " " + filters.SortDirection);
        else
            query = query.OrderByDescending(x => x.Createdon);

        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query.AsNoTracking().ToListAsync(ct);

        return new PaginatedList<WafRule>(items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<WafRule>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<WafRule>();

        var s = keyword.ToLower();

        return await _db.wafRules
            .Where(x => !x.Deleted &&
                       (x.RuleContent.ToLower().Contains(s) ||
                        x.Description.ToLower().Contains(s)))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<WafRule> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public IQueryable<WafRule> Query()
    {
        return _db.Set<WafRule>().AsQueryable();
    }
}