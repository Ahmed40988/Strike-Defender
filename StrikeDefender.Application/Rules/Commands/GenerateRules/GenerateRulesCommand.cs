using ErrorOr;
using MediatR;
using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Rules.RuleDTO;
using StrikeDefender.Domain.Common.Enums;

namespace StrikeDefender.Application.Rules.Commands.GenerateRules
{
    public record GenerateRulesCommand(string Prompt ) :IRequest<ErrorOr<TestGenerationRules>>;
}
