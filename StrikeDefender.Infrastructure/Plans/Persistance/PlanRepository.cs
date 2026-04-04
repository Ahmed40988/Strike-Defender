using ErrorOr;
using Microsoft.EntityFrameworkCore;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.Plans.PlansDTO;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Infrastructure.Common.Persistence.Data;
using System.Linq.Dynamic.Core;

namespace StrikeDefender.Infrastructure.Plans.Persistance;

public class PlanRepository(StrikeDefenderDbContext dbContext)
    : IGenericRepository<Plan>, IPlanRepository
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

    public Task<Plan> GetByName(string name)
    {
      return _db.Planes.FirstOrDefaultAsync(x => x.Name == name && !x.Deleted);
    }

    public async Task<ErrorOr<List<PlanResponse>>> GetAvailablePlansAsync(CancellationToken ct = default)
    {
       var plans= await _db.Planes
            .Where(x => !x.Deleted)
            .OrderBy(x => x.Price)
            .Select(x => new PlanResponse(
                x.Id,
                x.Name,
                x.Price,
                x.MaxAttacks,
                x.MaxRules,
                x.DurationInDays,
                x.MaxRiskScoreAccess
            ))
            .AsNoTracking()
            .ToListAsync(ct);
        if (plans == null || plans.Count == 0)
           return Error.NotFound("NoPlans", "No available plans found.");
        return plans;
    }

    public async Task AddRangeAsync(IEnumerable<Plan> entities)
    {
        await _db.AddRangeAsync(entities);
    }

    public IQueryable<Plan> Query()
    {
        return _db.Set<Plan>().AsQueryable();
    }
}