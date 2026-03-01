using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.APIs.Extensions;
using StrikeDefender.Application.subscriptions.Commands.Subscribe;
using StrikeDefender.Application.subscriptions.Commands.UpgradeSubscription;
using System.Security.Claims;


namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController(ISender Mediator) : ApiController
    {
        private readonly ISender _mediator = Mediator;


        /// <summary>
        /// Subscribe user to a selected plan
        /// </summary>
        /// <response code="200">Subscription created successfully</response>
        /// <response code="409">User already has an active subscription</response>
        /// <response code="404">Plan not found</response>
        [Authorize]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(Guid planId)
        {
            var userId = User.GetUserId();

            var result = await _mediator.Send(
                new SubscribeToPlanCommand(planId, userId!));

            return result.Match(
               _ => Ok(),
               errors => ToProblem(errors));
        }

        /// <summary>
        /// Upgrade user subscription to a new plan
        /// </summary>
        /// <response code="200">Subscription upgraded successfully</response>
        /// <response code="404">Subscription or plan not found</response>
        [Authorize]
        [HttpPost("upgrade_subscription")]
        public async Task<IActionResult> Upgrade(Guid planId)
        {
            var userId = User.GetUserId();

            var result = await _mediator.Send(
                new UpgradeSubscriptionCommand(planId, userId!));

            return result.Match(
        _ => Ok(),
        errors => ToProblem(errors));
        }
    }
}
