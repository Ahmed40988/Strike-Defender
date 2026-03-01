using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Application.Common.Interfaces;

public interface IAttackRepository
{
    Task AddAsync(Attack attack, CancellationToken cancellationToken = default);
    Task UpdateAsync(Attack attack, CancellationToken cancellationToken = default);
    Task DeleteAsync(Attack attack, CancellationToken cancellationToken = default);

    Task<Attack?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<Attack>> ListAsync(
        RequestFilters filters,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Attack>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default);
}