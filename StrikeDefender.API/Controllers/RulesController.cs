using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Rules.Commands.GenerateRules;
using StrikeDefender.Application.Rules.Commands.StoreSuccessfulRules;
using StrikeDefender.Application.Rules.RuleDTO;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController(ISender sender) : ApiController
    {
        private readonly ISender _Mediator = sender;

        [HttpPost("Generate_RulesFor_SuccessfulAttack")]
        public async Task<IActionResult> GenerateRules( CancellationToken ct)
        {
            var command = new GenerateRulesCommand(prompt);
            var result = await _Mediator.Send(command, ct);
            return result.Match(
                _ => Ok(_),
                errors => ToProblem(errors));
        }


        [HttpPost("Store_Successful_Rules")]
        public async Task<IActionResult> StoreRules( List<SuccessfulRulesDTO> rules, CancellationToken ct)
        {
            var command = new StoreSuccessfulRulesCommand(rules);
            var result = await _Mediator.Send(command, ct);
            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors));
        }




        string prompt = "You are a WAF rule generator.\n" +
"Input: 5 SQL injection payloads.\n" +
"Generate 1-3 ModSecurity rules.\n" +
"Use regex (@rx) with (?i), generalize patterns, avoid duplicates.\n" +
"Format:\n" +
"SecRule ARGS \"@rx pattern\" \"id:0,phase:2,deny,status:403,msg:'SQLi'\"\n" +
"Output: rules only, one per line.";
    }

    
}
