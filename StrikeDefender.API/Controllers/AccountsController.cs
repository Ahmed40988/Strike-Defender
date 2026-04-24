using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.APIs.Extensions;
using StrikeDefender.Application.Accounts.AccountDTO;
using StrikeDefender.Application.Accounts.Commands.CompleteProfile;
using StrikeDefender.Application.Accounts.Commands.GetProfile;

namespace StrikeDefender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(ISender sender) : ApiController
    {
        private readonly ISender _mediator = sender;

        [Authorize]
        [HttpPost("Complete-Profile")]
        public async Task<IActionResult> CompleteProfile(
         [FromForm] CreateProfileRequest Request)
        {
            var Userid = User.GetUserId();
            var commmand = new CompleteProfileCommand(Userid,
                Request.FullName, Request.Phone, Request.DateOfBirth, Request.Image);
            var result = await _mediator.Send(commmand);

            return result.Match(
                token => Ok(),
                errors => ToProblem(errors)
            );
        }

        [Authorize]
        [HttpGet("Get_Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var Userid = User.GetUserId();
            var commmand = new GetProfileCommand(Userid);
            var result = await _mediator.Send(commmand);
            return result.Match(
                profie => Ok(profie),
                errors => ToProblem(errors)
            );
        }







    }

    }
