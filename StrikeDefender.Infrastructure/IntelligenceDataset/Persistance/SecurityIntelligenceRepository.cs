using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.IntelligenceDataset;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.IntelligenceDataset.Persistance;

public class SecurityIntelligenceRepository(StrikeDefenderDbContext dbContext)
    : IGenericRepository<SecurityIntelligenceEntry>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(SecurityIntelligenceEntry entity, CancellationToken ct = default)
        => await _db.securityIntelligenceEntries.AddAsync(entity, ct);

    public Task UpdateAsync(SecurityIntelligenceEntry entity, CancellationToken ct = default)
    {
        _db.securityIntelligenceEntries.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SecurityIntelligenceEntry entity, CancellationToken ct = default)
    {
        _db.securityIntelligenceEntries.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<SecurityIntelligenceEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.securityIntelligenceEntries
            .Include(x => x.Attack)
            .Include(x => x.Rule)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.securityIntelligenceEntries
            .AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<SecurityIntelligenceEntry>> ListAsync(
        RequestFilters filters,
        CancellationToken ct = default)
    {
        var query = _db.securityIntelligenceEntries
            .Include(x => x.Attack)
            .Include(x => x.Rule)
            .Where(x => !x.Deleted)
            .AsQueryable();

        // 🔎 FULL SEARCH (payload + rule + ai + security)
        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();

            query = query.Where(x =>
                x.PayloadSnapshot.ToLower().Contains(s) ||
                x.RuleSnapshot.ToLower().Contains(s) ||
                x.AiAnalysis.ToLower().Contains(s) ||
                x.SecurityAnalysis.ToLower().Contains(s));
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

        return new PaginatedList<SecurityIntelligenceEntry>(
            items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<SecurityIntelligenceEntry>> SearchAsync(
        string keyword,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<SecurityIntelligenceEntry>();

        var s = keyword.ToLower();

        return await _db.securityIntelligenceEntries
            .Where(x => !x.Deleted &&
                       (x.PayloadSnapshot.ToLower().Contains(s) ||
                        x.RuleSnapshot.ToLower().Contains(s) ||
                        x.AiAnalysis.ToLower().Contains(s)))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<SecurityIntelligenceEntry> entities)
    {
       await _db.securityIntelligenceEntries.AddRangeAsync(entities);
    }
}