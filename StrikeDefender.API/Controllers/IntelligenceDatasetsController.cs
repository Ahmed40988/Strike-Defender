using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.APIs.Extensions;
using StrikeDefender.Application.Common.Pagination;
using StrikeDefender.Application.IntelligenceDatasets.Queries.GetSecurityDataset;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntelligenceDatasetsController(ISender Mediator) : ApiController
    {
        private readonly ISender _mediator = Mediator;


        /// <summary>
        /// Get security intelligence dataset based on user subscription plan
        /// </summary>
        /// <remarks>
        /// Returns filtered intelligence data depending on the user's active subscription.
        /// Users only see data allowed by their plan risk score.
        /// </remarks>
        /// <response code="200">Get data successfully</response>
        /// <response code="400">Invalid request or parameters</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">Subscription expired or inactive</response>
        /// <response code="404">No dataset found for this plan</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("Get_Intelligence_Datasets")]
        [Authorize]
        public IActionResult GetIntelligenceDatasets([FromQuery] RequestFilters requestFilters )
        {
            var UserId=User.GetUserId();
            var query = new GetSecurityDatasetQuery(UserId, requestFilters);
            var result = _mediator.Send(query).Result;
            return result.Match(
             _ => Ok(_),
             errors => ToProblem(errors)
         );



        }
    }
}
