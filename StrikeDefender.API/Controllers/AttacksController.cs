using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Attacks.AttackDTO;
using StrikeDefender.Application.Attacks.Commands.GenerateAttacks;
using StrikeDefender.Application.Attacks.Commands.StoreSuccessfulAttacks;
using StrikeDefender.Domain.Common.Enums;
using System.Text.Json;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttacksController(ISender Mediator) : ApiController
    {
        private readonly ISender _mediator = Mediator;

        [HttpPost("Test_Prompts")]
        public async Task<IActionResult> GenerateAttacks([FromBody] GenerateAttackCommand request, CancellationToken ct)
        {
            var command = new GenerateAttackCommand(request.Prompt, AttackType.SQLInjection);
            var result = await _mediator.Send(command, ct);
            return result.Match(
                attacks => Ok(attacks),
                errors => ToProblem(errors));
        }

        [HttpPost("generate_Atttacks_Static_Prompt")]
        public async Task<IActionResult> GenerateAttacksByStaticPrompt( CancellationToken ct)
        {
            var command = new GenerateAttackCommand(Prompt, AttackType.SQLInjection);
            var result = await _mediator.Send(command, ct);
            return result.Match(
                attacks => Ok(attacks),
                errors => ToProblem(errors));
        }

        /// <summary>
        /// Import successful attacks from sandbox execution engine
        /// </summary>
        /// <response code="200">Successful attacks stored successfully</response>
        /// <response code="400">Invalid attack payload format</response>
        /// <response code="401">Unauthorized sandbox request</response>
        /// <response code="404">Referenced attack not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("import-successful-attacks")]
        public async Task<IActionResult> ImportSuccessfulAttacks( [FromBody] List<SuccessfulAttackDto> attacks, CancellationToken cancellationToken)
        {
            var command = new StoreSuccessfulAttacksCommand(attacks);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match(
               _ => Ok(),
               errors => ToProblem(errors));
        }

        string Prompt = "You are a cybersecurity dataset generator for training input validation models.\nYour task is to generate 50 synthetic SQL injection input strings for a MySQL/MariaDB target.\nTechnical Context:\nDatabase: MySQL / MariaDB only\nInjection point: GET parameter id inside single quotes → WHERE user_id = '$id'\nPayloads must start with ' to break out of the string context\nUNION-based payloads must return exactly 2 columns (matching first_name, last_name)\nUse MySQL comment syntax only: # or --  (with trailing space)\nAllowed functions: database(), version(), user(), SLEEP(), BENCHMARK(), information_schema\nDo NOT use SQL Server syntax: no xp_cmdshell, WAITFOR DELAY, or EXEC\nDo NOT include destructive queries: no DROP, DELETE, or UPDATE\nGeneration Rules:\nMix all SQLi styles: error-based, union-based, boolean-based, time-based, and stacked queries\nVary length and complexity: short, medium, and long payloads\nInclude realistic-looking and real working payloads\nNo duplicates\nGoals simulated:\nAuthentication bypass\nDatabase name and version extraction\nMetadata extraction via information_schema\nTime-based blind injection\nAttacker behavior simulation for testing and training\nOutput format:\nA single clean JSON array of exactly 50 strings\n No explanations, no comments, no markdown — raw JSON only\nNow generate the 50 synthetic SQL injection test patterns.";
   
}
    }
