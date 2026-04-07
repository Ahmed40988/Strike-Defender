using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Rules.Commands.GenerateRules
{
    public class GenerateRulesCommandValidator:AbstractValidator<GenerateRulesCommand>
    {
        public GenerateRulesCommandValidator()
        {
            
        }

    }
}
