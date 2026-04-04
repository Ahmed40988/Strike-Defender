using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Application.Common.Interfaces;

public interface IGenericRepository<T>
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<T>> ListAsync(
        RequestFilters filters,
        CancellationToken cancellationToken = default);

    IQueryable<T> Query();

    Task<IReadOnlyList<T>> SearchAsync(
        string keyword,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities);


   
}