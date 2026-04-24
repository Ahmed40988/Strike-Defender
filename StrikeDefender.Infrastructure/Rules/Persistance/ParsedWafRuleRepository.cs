using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Rules;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Rules.Persistance;

public class ParsedWafRuleRepository(StrikeDefenderDbContext dbContext) : IGenericRepository<ParsedWafRule>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(ParsedWafRule entity, CancellationToken ct = default)
        => await _db.Set<ParsedWafRule>().AddAsync(entity, ct);

    public Task UpdateAsync(ParsedWafRule entity, CancellationToken ct = default)
    {
        _db.Set<ParsedWafRule>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ParsedWafRule entity, CancellationToken ct = default)
    {
        _db.Set<ParsedWafRule>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<ParsedWafRule?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<ParsedWafRule>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<ParsedWafRule>()
            .AnyAsync(x => x.Id == id, ct);

    public async Task<PaginatedList<ParsedWafRule>> ListAsync(RequestFilters filters, CancellationToken ct = default)
    {
        var query = _db.Set<ParsedWafRule>()
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();

            query = query.Where(x =>
                x.Pattern.ToLower().Contains(s) ||
                x.Message.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy(filters.SortColumn + " " + filters.SortDirection);
        else
            query = query.OrderByDescending(x => x.RuleId);

        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query.AsNoTracking().ToListAsync(ct);

        return new PaginatedList<ParsedWafRule>(items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<ParsedWafRule>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<ParsedWafRule>();

        var s = keyword.ToLower();

        return await _db.Set<ParsedWafRule>()
            .Where(x =>
                x.Pattern.ToLower().Contains(s) ||
                x.Message.ToLower().Contains(s))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<ParsedWafRule> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public IQueryable<ParsedWafRule> Query()
    {
        return _db.Set<ParsedWafRule>().AsQueryable();
    }
}