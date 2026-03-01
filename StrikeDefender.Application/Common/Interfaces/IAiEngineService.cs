using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IAiEngineService
    {
        Task<ErrorOr<List<string>>> GenerateAttacksAsync(
      string prompt,
      CancellationToken ct = default);
    }
}
