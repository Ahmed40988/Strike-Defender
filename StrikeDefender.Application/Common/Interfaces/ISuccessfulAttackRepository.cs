using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;

namespace StrikeDefender.Application.Common.Interfaces;

public interface ISuccessfulAttackRepository
{

    Task<List<SuccessfulAttack>> GetByIdsAsync(
    List<Guid> Ids,
    CancellationToken cancellationToken);

}