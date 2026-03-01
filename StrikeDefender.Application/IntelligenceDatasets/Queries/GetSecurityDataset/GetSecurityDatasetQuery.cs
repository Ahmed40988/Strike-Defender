using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.IntelligenceDatasets.IntelligenceDatasetsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.IntelligenceDatasets.Queries.GetSecurityDataset
{
    public record GetSecurityDatasetQuery(
        string UserId,
        RequestFilters Filters)
    : IRequest<ErrorOr<PaginatedList<SecurityEntryResponse>>>;
}
