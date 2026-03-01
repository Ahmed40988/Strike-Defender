using StrikeDefender.Application.Plans.PlansDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Plans.Queries.GetPlans
{
    public record GetPlansQuery()
       : IRequest<ErrorOr<List<PlanResponse>>>;
}
