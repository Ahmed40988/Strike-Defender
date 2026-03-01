using StrikeDefender.Application.Plans.PlansDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Plans.Queries.GetPlans
{
    public class GetPlansQueryHandler(IPlanRepository planRepo)
     : IRequestHandler<GetPlansQuery, ErrorOr<List<PlanResponse>>>
    {
        private readonly IPlanRepository _planRepo = planRepo;

        public async Task<ErrorOr<List<PlanResponse>>> Handle(
            GetPlansQuery request,
            CancellationToken cancellationToken)
        {
            return await _planRepo.GetAvailablePlansAsync(cancellationToken);
        }
    }
}
