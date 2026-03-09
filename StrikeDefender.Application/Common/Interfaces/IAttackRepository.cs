using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Application.Common.Interfaces;

public interface IAttackRepository
{

    Task<List<Attack>> GetByIdsAsync(
    List<Guid> Ids,
    CancellationToken cancellationToken);

}