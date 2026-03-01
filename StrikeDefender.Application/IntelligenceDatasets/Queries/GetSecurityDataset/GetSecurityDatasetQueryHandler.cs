using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.IntelligenceDatasets.IntelligenceDatasetsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.IntelligenceDatasets.Queries.GetSecurityDataset
{
    public class GetSecurityDatasetQueryHandler(ISubscriptionAccessService subscriptionAccess) : IRequestHandler<GetSecurityDatasetQuery, ErrorOr<PaginatedList<SecurityEntryResponse>>>
    {
        private readonly ISubscriptionAccessService _subscriptionAccess = subscriptionAccess;

        public async Task<ErrorOr<PaginatedList<SecurityEntryResponse>>> Handle(
      GetSecurityDatasetQuery request,
      CancellationToken cancellationToken)
        {
            return await _subscriptionAccess
                .GetDatasetForUserAsync(request.UserId, request.Filters, cancellationToken);
        }
    }
}
