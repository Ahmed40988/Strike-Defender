using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Domain.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Attacks.Queries.GetSuccessfulAttacks
{
    public class GetSuccessfulAttacksQueryHandler(IGenericRepository<SuccessfulAttack> genericRepository) :
        IRequestHandler<GetSuccessfulAttacksQuery, ErrorOr<PaginatedList<SuccessfulAttackDto>>>
    {
        private readonly IGenericRepository<SuccessfulAttack> _genericRepository = genericRepository;

        public async Task<ErrorOr<PaginatedList<SuccessfulAttackDto>>> Handle(
           GetSuccessfulAttacksQuery request,
           CancellationToken cancellationToken)
        {
            var filters = request.Filters;

            var query = _genericRepository.Query()
                .Include(x => x.Attack)
                .Include(x => x.AttackResult)
                .Where(x => !x.Deleted)
                .OrderByDescending(x => x.Createdon);

            // ✅ هنا بنعمل pagination + execution
            var result = await PaginatedList<SuccessfulAttack>.CreateAsync(
                query,
                filters.PageNumber,
                filters.PageSize,
                cancellationToken
            );

            // ✅ mapping بعد ما جبت data
            var mappedItems = result.Items
                .Select(x => new SuccessfulAttackDto(
                    x.AttackId,
                    x.Attack.Payload,
                    x.Target,
                    x.BypassTechnique,
                    x.Severity,
                    x.AttackResult.IsBlockedByWaf,
                    x.AttackResult.ResponseCode,
                    x.AttackResult.ResponseBody,
                    x.AttackResult.ExecutionTimeMs
                ))
                .ToList();

            return new PaginatedList<SuccessfulAttackDto>(
                mappedItems,
                result.PageNumber,
                query.Count(), 
                filters.PageSize
            );
        }
    }
}

