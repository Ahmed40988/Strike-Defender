using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Plans.PlansDTO
{
    public record PlanResponse
    (
        Guid Id,
        string Name,
        decimal Price,
        int MaxAttacks,
        int MaxRules,
        int DurationInDays,
        int MaxRiskScoreAccess
    );
}


