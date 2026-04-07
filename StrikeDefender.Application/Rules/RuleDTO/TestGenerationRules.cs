using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Rules.RuleDTO
{
    public record TestGenerationRules(
        List<RuleWithIdDto> Rules,
        List<AttackPayloadDto> Attacks);

    public record RuleWithIdDto(
        Guid RuleId,
        string RuleContent);

    public record AttackPayloadDto(
        Guid Id,
        string Payload);
}
