using StrikeDefender.Application.Attacks.AttackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Attacks.Commands.StoreSuccessfulAttacks
{
    public record StoreSuccessfulAttacksCommand(
        List<SuccessfulAttackDto> Attacks
    ) : IRequest<ErrorOr<Success>>;
}
