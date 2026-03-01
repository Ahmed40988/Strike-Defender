using StrikeDefender.Application.Plans.PlansDTO;
using StrikeDefender.Domain.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IPlanRepository
    {
        Task<Plan>GetByName(string name);
        Task<ErrorOr<List<PlanResponse>>>GetAvailablePlansAsync(CancellationToken ct = default);
    }
}
