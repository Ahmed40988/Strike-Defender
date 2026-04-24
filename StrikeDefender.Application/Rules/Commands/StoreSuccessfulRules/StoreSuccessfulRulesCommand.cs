using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Rules.RuleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Rules.Commands.StoreSuccessfulRules
{
    public record StoreSuccessfulRulesCommand(
        List<SuccessfulRulesDTO> Rules
    ) : IRequest<ErrorOr<Success>>;
}
