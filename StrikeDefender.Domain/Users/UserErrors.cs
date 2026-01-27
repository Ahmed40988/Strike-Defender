using ErrorOr;

namespace StrikeDefender.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials = Error.Unauthorized(
            code: "User.InvalidCredentials",
            description: "Invalid email or password.");

        public static readonly Error UserNotFound = Error.NotFound(
            code: "User.NotFound",
            description: "No user found with the given email or ID.");

        public static readonly Error DuplicatedEmail = Error.Conflict(
            code: "User.DuplicatedEmail",
            description: "Another user with the same email already exists.");

        public static readonly Error EmailNotConfirmed = Error.Unauthorized(
            code: "User.EmailNotConfirmed",
            description: "Email is not confirmed yet.");

        public static readonly Error PasswordMismatch = Error.Validation(
            code: "User.PasswordMismatch",
            description: "New password and confirmation do not match.");

        public static readonly Error ResetPasswordFailed = Error.Failure(
            code: "User.ResetPasswordFailed",
            description: "Failed to reset the password.");

        public static readonly Error RegisterFailed = Error.Failure(
            code: "User.RegisterFailed",
            description: "Failed to register new user.");

        public static readonly Error DisabledUser = Error.Unauthorized(
            code: "User.DisabledUser",
            description: "Disabled user. Please contact your administrator.");

        public static readonly Error LockedUser = Error.Unauthorized(
            code: "User.LockedUser",
            description: "Locked user. Please contact your administrator.");

        public static readonly Error InvalidJwtToken = Error.Unauthorized(
            code: "User.InvalidJwtToken",
            description: "Invalid JWT token.");

        public static readonly Error InvalidRefreshToken = Error.Unauthorized(
            code: "User.InvalidRefreshToken",
            description: "Invalid refresh token.");

        public static readonly Error InvalidOTP = Error.Unauthorized(
            code: "User.InvalidOTP",
            description: "Invalid OTP code. Please enter the correct code.");

        public static readonly Error OTPExpired = Error.Unauthorized(
            code: "User.OTPExpired",
            description: "OTP expired or not found. Please request a new one.");

        public static readonly Error DuplicatedConfirmation = Error.Conflict(
            code: "User.DuplicatedConfirmation",
            description: "Email already confirmed.");

        public static readonly Error UnexpectedServerError = Error.Failure(
            code: "Server.Error",
            description: "An unexpected error occurred. Please try again later.");

        public static readonly Error UpdateFailed = Error.Failure(
            code: "User.UpdateFailed",
            description: "Failed to update user information.");

        public static readonly Error DeleteFailed = Error.Failure(
            code: "User.DeleteFailed",
            description: "Failed to delete user.");

        public static readonly Error LockFailed = Error.Failure(
            code: "User.LockFailed",
            description: "Failed to lock user account.");

        public static readonly Error UnlockFailed = Error.Failure(
            code: "User.UnlockFailed",
            description: "Failed to unlock user account.");
    }
}
