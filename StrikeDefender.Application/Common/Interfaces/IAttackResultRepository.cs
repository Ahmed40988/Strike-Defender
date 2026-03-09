using StrikeDefender.Domain.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IAttackResultRepository
    {
        Task AddRangeAsync(IEnumerable<AttackResult> results,
      CancellationToken cancellationToken = default);

    }
}
