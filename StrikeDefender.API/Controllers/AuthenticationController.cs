
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrikeDefender.API.Controllers;
using StrikeDefender.APIs.Extensions;
using StrikeDefender.Application.Accounts.AccountDTO;
using StrikeDefender.Application.Accounts.Commands.CompleteProfile;
using StrikeDefender.Application.Accounts.Commands.ConfirmEmail;
using StrikeDefender.Application.Accounts.Commands.CreateAccount;
using StrikeDefender.Application.Accounts.Commands.ForgotPassword;
using StrikeDefender.Application.Accounts.Commands.Login;
using StrikeDefender.Application.Accounts.Commands.RefreshToken;
using StrikeDefender.Application.Accounts.Commands.ResendConfirmationEmail;
using StrikeDefender.Application.Accounts.Commands.ResetPassword;
using StrikeDefender.Application.Accounts.Commands.VerifyForgotPasswordOtp;
using StrikeDefender.Application.Accounts.AccountDTO;
using WStrikeDefendereb.Application.Accounts.AccountDTO;

namespace StrikeDefender.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(ISender mediator) : ApiController
    {
        private readonly ISender _mediator = mediator;

        /// <summary>
        /// Register a new user using email and password.
        /// </summary>
        /// <remarks>
        /// Creates a new account and sends a confirmation OTP to the user's email.
        /// </remarks>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Validation error</response>
        /// <response code="409">Email already exists</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            var command = new CreateAccountCommand(
                request.Email,
                request.Password
            );

            var result = await _mediator.Send(command);

            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Resend email confirmation OTP.
        /// </summary>
        /// <remarks>
        /// Sends a new confirmation OTP if the email is not confirmed yet.
        /// </remarks>
        /// <response code="200">OTP sent successfully</response>
        /// <response code="409">Email already confirmed</response>
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(
            [FromBody] ResendConfirmEmailRequest request)
        {
            var command = new ResendConfirmationEmailCommand(request.Email);

            var result = await _mediator.Send(command);

            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Confirm user email using OTP.
        /// </summary>
        /// <response code="200">Email confirmed successfully</response>
        /// <response code="400">Invalid or expired OTP</response>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request)
        {
            var command = new ConfirmEmailCommand(
                request.email,
                request.OTP
            );

            var result = await _mediator.Send(command);

            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Login using email and password.
        /// </summary>
        /// <response code="200">JWT and Refresh Token returned</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="403">Email not confirmed or user locked</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var command = new LoginCommand(
                request.Email,
                request.Password
            );

            var result = await _mediator.Send(command);

            return result.Match(
                token => Ok(token),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Send OTP to reset password.
        /// </summary>
        /// <remarks>
        /// An OTP will be sent to the registered email address.
        /// </remarks>
        /// <response code="200" >OTP sent successfully</response>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgetPasswordDto request)
        {
            var command = new ForgotPasswordCommand(request.Email);

            var result = await _mediator.Send(command);

            return result.Match(
                message => Ok(new { message }),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Verify OTP for forgot password.
        /// </summary>
        /// <remarks>
        /// Returns a password reset token if OTP is valid.
        /// </remarks>
        /// <response code="200">OTP verified successfully</response>
        /// <response code="400">Invalid or expired OTP</response>
        [HttpPost("verify-forgot-password-otp")]
        public async Task<IActionResult> VerifyForgotPasswordOtp(
            [FromBody] VerfiyCodeDto request)
        {
            var command = new VerifyForgotPasswordOtpCommand(
                request.Email,
                request.CodeOTP
            );

            var result = await _mediator.Send(command);

            return result.Match(
                token => Ok(new { resetToken = token }),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Reset user password.
        /// </summary>
        /// <remarks>
        /// Requires a valid reset token returned from OTP verification.
        /// </remarks>
        /// <response code="200">Password reset successfully</response>
        /// <response code="400">Invalid token or password mismatch</response>
        /// <response code="404">Enter Correct Values!</response>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto request)
        {
            var command = new ResetPasswordCommand(
                request.Email,
                request.Token,
                request.NewPassword,
                request.ConfirmNewPassword
            );

            var result = await _mediator.Send(command);

            return result.Match(
                _ => Ok(),
                errors => ToProblem(errors)
            );
        }

        /// <summary> Refresh JWT token using refresh token. </summary>
        /// <remarks> Returns a new JWT and refresh token. </remarks>
        /// <response code="200">New tokens generated</response>
        /// <response code="400">Invalid token or refresh token</response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand(
                request.Token,
                request.RefreshToken
            );

            var result = await _mediator.Send(command);

            return result.Match(
                token => Ok(token),
                errors => ToProblem(errors)
            );
        }

        /// <summary>
        /// Complete user profile information. </summary>
        /// <remarks>
        /// The request must be sent With Token as need Userid from Token <br/><br/>
        /// The request must be sent as <b>multipart/form-data</b> because it supports file upload. </remarks>
        /// <response code="200">Profile completed successfully. </response>
        /// <response code="400">Invalid request data or validation error. </response>
        /// <response code="401"> Unauthorized. JWT token is missing or invalid. </response>
        /// <response code="404"> User not found. </response>

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
    }
}
