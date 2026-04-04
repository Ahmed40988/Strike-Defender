using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Attacks.Queries.GetSuccessfulAttacks
{
    public record GetSuccessfulAttacksQuery(BaseFilters Filters):
        IRequest<ErrorOr<PaginatedList<SuccessfulAttackDto>>>;
}
