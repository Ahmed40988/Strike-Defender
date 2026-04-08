using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IAiProvider
    {
        string Name { get; } //logging
    Task<ErrorOr<List<string>>> SendAsync(
            string prompt,
            CancellationToken ct);
    }
}
