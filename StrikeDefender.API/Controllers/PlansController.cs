using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Plans.Queries.GetPlans;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController(ISender Mediator) : ApiController
    {
        private readonly ISender _mediator = Mediator;

        /// <summary>
        /// Get all available subscription plans
        /// </summary>
        /// <response code="200">Plans retrieved successfully</response>
        /// <response code="404">No available plans found</response>
        [HttpGet("Get_Plans")]
        public async Task<IActionResult> GetPlans()
        {
            var result = await _mediator.Send(new GetPlansQuery());
            return result.Match(
             Plans => Ok(Plans),
             errors => ToProblem(errors));
        }
    }
}
