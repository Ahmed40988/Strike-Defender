using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Attacks.Commands.GenerateAttacks;
using System.Text.Json;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttacksController(ISender Mediator) : ApiController
    {
        private readonly ISender _mediator = Mediator;


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAttacks([FromBody] GenerateAttackCommand request, CancellationToken ct)
        {
            var command = new GenerateAttackCommand(request.Prompt);
            var result = await _mediator.Send(command, ct);
            return result.Match(
                attacks => Ok(attacks),
                errors => ToProblem(errors));
        }
    

    }
    }
