using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IRuleRepository
    {
        Task<List<WafRule>> GetByIdsAsync(
List<Guid> Ids,
CancellationToken cancellationToken);

    }
}
