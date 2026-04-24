using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Rules.RuleDTO
{
    public record SuccessfulRulesDTO(
        Guid attackId,
        Guid ruleId
    );
}
