
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Infrastructure.Common.Persistence.Data;

namespace StrikeDefender.Infrastructure.SuccessfulAttacks.Persistance;

public class SuccessfulAttackRepository(StrikeDefenderDbContext dbContext) : IGenericRepository<SuccessfulAttack>
{
    private readonly StrikeDefenderDbContext _db = dbContext;

    public async Task AddAsync(SuccessfulAttack entity, CancellationToken ct = default)
        => await _db.SuccessfulAttacks.AddAsync(entity, ct);

    public Task UpdateAsync(SuccessfulAttack entity, CancellationToken ct = default)
    {
        _db.SuccessfulAttacks.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SuccessfulAttack entity, CancellationToken ct = default)
    {
        _db.SuccessfulAttacks.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<SuccessfulAttack?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.SuccessfulAttacks
            .Include(x => x.Attack)
            .Include(x => x.AttackResult)
            .FirstOrDefaultAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _db.SuccessfulAttacks.AnyAsync(x => x.Id == id && !x.Deleted, ct);

    public async Task<PaginatedList<SuccessfulAttack>> ListAsync(RequestFilters filters, CancellationToken ct = default)
    {
        var query = _db.SuccessfulAttacks
            .Include(x => x.Attack)
            .Include(x => x.AttackResult)
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

        return new PaginatedList<SuccessfulAttack>(items, filters.PageNumber, total, filters.PageSize);
    }

    public async Task<IReadOnlyList<SuccessfulAttack>> SearchAsync(string keyword, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<SuccessfulAttack>();

        var s = keyword.ToLower();

        return await _db.SuccessfulAttacks
            .Include(x => x.Attack)
            .Where(x => !x.Deleted &&
                       (x.BypassTechnique.ToLower().Contains(s) ||
                        x.Attack.Payload.ToLower().Contains(s)))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
