using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.IntelligenceDatasets.IntelligenceDatasetsDTO;
using StrikeDefender.Domain.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Interfaces
{
   public interface ISubscriptionAccessService
    {
        Task<ErrorOr<PaginatedList<SecurityEntryResponse>>>GetDatasetForUserAsync(string userId, RequestFilters filters, CancellationToken ct = default);
        Task<bool> HasActiveSubscriptionAsync(string userId);
        Task<int> GetUserMaxRiskScoreAsync(string userId);
        Task<Subscription> FirstOrDefaultByUserIdAsync(string userId);
    }
}
