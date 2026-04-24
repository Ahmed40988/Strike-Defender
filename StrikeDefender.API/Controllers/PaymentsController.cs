using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.Application.Payments.Commands.PaymentWebhook;
using StrikeDefender.Application.Payments.DTO;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(ISender sender) : ApiController
    {
        private readonly ISender _mediator = sender;

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] PaymobWebhookRequest request)
        {
            var result = await _mediator.Send(
                new HandlePaymentWebhookCommand(request));

            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors));
        }
    }
}
