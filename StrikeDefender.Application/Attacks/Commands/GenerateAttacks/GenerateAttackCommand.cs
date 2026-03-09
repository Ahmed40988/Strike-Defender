using ErrorOr;
using MediatR;
using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Domain.Common.Enums;

namespace StrikeDefender.Application.Attacks.Commands.GenerateAttacks
{
    public record GenerateAttackCommand(string Prompt, AttackType Type ) :IRequest<ErrorOr<List<AttackResponce>>>;
}
