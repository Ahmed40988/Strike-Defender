using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Attacks.Commands.GenerateAttacks
{
    public class GenerateAttackCommandValidator:AbstractValidator<GenerateAttackCommand>
    {
        public GenerateAttackCommandValidator()
        {
            
        }

    }
}
