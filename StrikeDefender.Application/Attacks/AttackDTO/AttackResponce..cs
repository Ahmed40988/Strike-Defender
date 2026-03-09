using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Attacks.AttackDTO
{
    public record AttackResponce(
    Guid Id,
    string Payload
);
}