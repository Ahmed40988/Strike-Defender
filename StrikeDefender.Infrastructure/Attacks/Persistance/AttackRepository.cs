using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Attacks.Persistance;

public class AttackRepository(StrikeDefenderDbContext dbContext)
    : IAttackRepository
{
    private readonly StrikeDefenderDbContext _dbContext = dbContext;

    public async Task AddAsync(Attack attack, CancellationToken cancellationToken = default)
    {
        await _dbContext.Attacks.AddAsync(attack, cancellationToken);
    }

    public Task UpdateAsync(Attack attack, CancellationToken cancellationToken = default)
    {
        _dbContext.Attacks.Update(attack);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Attack attack, CancellationToken cancellationToken = default)
    {
        _dbContext.Attacks.Remove(attack);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attacks
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && !x.Deleted, cancellationToken);
    }

    public async Task<Attack?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attacks
            .Include(x => x.Rule)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, cancellationToken);
    }

    public async Task<PaginatedList<Attack>> ListAsync(
        RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Attacks
            .Include(x => x.Rule)
            .Where(x => !x.Deleted)
            .AsNoTracking()
            .AsQueryable();

        // 🔎 search
        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var search = filters.SearchValue.ToLower();

            query = query.Where(x =>
                x.Payload.ToLower().Contains(search) ||
                x.Target.ToLower().Contains(search) ||
                x.Rule.RuleContent.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // sorting
        if (!string.IsNullOrEmpty(filters.SortColumn))
        {
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");
        }
        else
        {
            query = query.OrderByDescending(x => x.Createdon);
        }

        // pagination
        if (filters.PageNumber > 0 && filters.PageSize > 0)
        {
            var skip = (filters.PageNumber - 1) * filters.PageSize;
            query = query.Skip(skip).Take(filters.PageSize);
        }

        var items = await query.ToListAsync(cancellationToken);

        return new PaginatedList<Attack>(
            items,
            filters.PageNumber,
            totalCount,
            filters.PageSize);
    }

    public async Task<IReadOnlyList<Attack>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<Attack>();

        var lower = keyword.ToLower();

        return await _dbContext.Attacks
            .Include(x => x.Rule)
            .Where(x => !x.Deleted &&
                       (x.Payload.ToLower().Contains(lower) ||
                        x.Target.ToLower().Contains(lower) ||
                        x.Rule.RuleContent.ToLower().Contains(lower)))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}