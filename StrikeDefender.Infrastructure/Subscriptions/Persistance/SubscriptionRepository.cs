using ErrorOr;
using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.IntelligenceDatasets.IntelligenceDatasetsDTO;
using StrikeDefender.Domain.IntelligenceDataset;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Subscriptions.Persistance;

public class SubscriptionRepository(StrikeDefenderDbContext dbContext)
    : IGenericRepository<Subscription>, ISubscriptionAccessService
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(Subscription entity, CancellationToken ct = default)
        => await _db.Subscriptions.AddAsync(entity, ct);

    public Task UpdateAsync(Subscription entity, CancellationToken ct = default)
    {
        _db.Subscriptions.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Subscription entity, CancellationToken ct = default)
    {
        _db.Subscriptions.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Subscriptions
            .Include(x => x.Plan)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.Subscriptions
            .AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<Subscription>> ListAsync(
        RequestFilters filters,
        CancellationToken ct = default)
    {
        var query = _db.Subscriptions
            .Include(x => x.Plan)
            .Include(x => x.User)
            .Where(x => !x.Deleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();

            query = query.Where(x =>
                x.Plan.Name.ToLower().Contains(s) ||
                x.User.Id.ToString().ToLower().Contains(s));
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

        return new PaginatedList<Subscription>(
            items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<Subscription>> SearchAsync(
        string keyword,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<Subscription>();

        var s = keyword.ToLower();

        return await _db.Subscriptions
            .Include(x => x.Plan)
            .Where(x => !x.Deleted &&
                       x.Plan.Name.ToLower().Contains(s))
            .AsNoTracking()
            .ToListAsync(ct);
    }
    public async Task<ErrorOr<PaginatedList<SecurityEntryResponse>>>
GetDatasetForUserAsync(string userId, RequestFilters filters, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return SecurityIntelligenceEntryErrors.InvalidUser;

        var sub = await _db.Subscriptions
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.Deleted, ct);

        if (sub is null)
            return SubscriptionErrors.NotFound;

        if (!sub.IsActive)
            return SubscriptionErrors.Inactive;

        if (sub.IsExpired())
            return SubscriptionErrors.Expired;

        var maxRisk = sub.Plan.MaxRiskScoreAccess;

        // 🔥 base query
        var query = _db.securityIntelligenceEntries
            .Where(x => !x.Deleted && x.RiskScore <= maxRisk)
            .AsQueryable();

        // 🔍 search
        if (!string.IsNullOrEmpty(filters.SearchValue))
        {
            var s = filters.SearchValue.ToLower();

            query = query.Where(x =>
                x.AttackType.ToString().ToLower().Contains(s) ||
                x.Severity.ToString().ToLower().Contains(s) ||
                x.AiAnalysis.ToLower().Contains(s));
        }

        var total = await query.CountAsync(ct);

        if (total == 0)
            return SecurityIntelligenceEntryErrors.NoData;

        // 🔽 sorting
        if (!string.IsNullOrEmpty(filters.SortColumn))
            query = query.OrderBy(filters.SortColumn + " " + filters.SortDirection);
        else
            query = query.OrderByDescending(x => x.Createdon);

        // 📄 pagination
        if (filters.PageNumber > 0 && filters.PageSize > 0)
            query = query.Skip((filters.PageNumber - 1) * filters.PageSize)
                         .Take(filters.PageSize);

        var items = await query
            .Select(x => new SecurityEntryResponse(
                x.Id,
                x.PayloadSnapshot,
                x.TargetSnapshot,
                x.RuleSnapshot,
                x.AttackType.ToString(),
                x.Severity.ToString(),
                x.RiskScore,
                x.AiAnalysis,
                x.SecurityAnalysis
            ))
            .AsNoTracking()
            .ToListAsync(ct);

        var paginated = new PaginatedList<SecurityEntryResponse>(
            items,
            filters.PageNumber,
            total,
            filters.PageSize);

        return paginated;
    }
    public async Task<bool> HasActiveSubscriptionAsync(string userId)
    {
        var sub = await _db.Subscriptions
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.Deleted);

        if (sub is null)
            return false;

        return sub.HasAccess();
    }

    public async Task<int> GetUserMaxRiskScoreAsync(string userId)
    {
        var sub = await _db.Subscriptions
            .Include(x => x.Plan)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.Deleted);

        if (sub is null || !sub.HasAccess())
            return 0;

        return sub.Plan.MaxRiskScoreAccess;
    }

    public async Task<Subscription> FirstOrDefaultByUserIdAsync(string userId)
    {
        return await _db.Subscriptions
            .Include(x => x.Plan)
     .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive&& !x.Deleted);
    }

    public async Task AddRangeAsync(IEnumerable<Subscription> entities)
    {
      await _db.Subscriptions.AddRangeAsync(entities);
    }

    public IQueryable<Subscription> Query()
    {
        return _db.Set<Subscription>().AsQueryable();
    }
}

