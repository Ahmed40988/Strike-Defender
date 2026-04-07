using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Dashboard.Queries.DeleteUser;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(ISender sender) : ApiController
    {
        private readonly ISender _Mediator = sender;

        [HttpDelete("users/{email}")]
        public async Task<IActionResult> DeleteUserByEmail(string email, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new DeleteUserbyEmailQuery(email), cancellationToken);

            return result.Match(
                deleted => Ok(),
                errors => ToProblem(errors));
        }
    }
}
