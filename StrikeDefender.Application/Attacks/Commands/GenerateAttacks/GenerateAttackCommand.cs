using ErrorOr;
using MediatR;
using StrikeDefender.Application.Attacks.AttackDTO;

namespace StrikeDefender.Application.Attacks.Commands.GenerateAttacks
{
    public record GenerateAttackCommand(string Prompt):IRequest<ErrorOr<List<string>>>;
}
