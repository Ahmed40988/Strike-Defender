// Application/Common/Interfaces/IIdentityService.cs
using ErrorOr;

namespace StrikeDefender.Application.Common.Interfaces
{
    public interface IIdentityService
    {
 
        // ─── Find ───────────────────────────────────────────
        Task<ErrorOr<string>> FindUserIdByEmailAsync(string email);

        // ─── Create / Delete / Restore ──────────────────────
        Task<ErrorOr<string>> CreateUserAsync(string email, string password);
        Task<ErrorOr<Success>> DeleteUserAsync(string userId);
        Task<ErrorOr<Success>> RestoreUserAsync(string email, string newPassword);

        // ─── Password ────────────────────────────────────────
        Task<ErrorOr<Success>> CheckPasswordAsync(string userId, string password);
        Task<ErrorOr<Success>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<ErrorOr<Success>> ResetPasswordAsync(string userId, string newPassword);
        Task<ErrorOr<string>> GeneratePasswordResetTokenAsync(string userId);

        // ─── Email Confirmation ──────────────────────────────
        Task<ErrorOr<Success>> ConfirmEmailAsync(string userId, string token);
        Task<ErrorOr<string>> GenerateEmailConfirmationTokenAsync(string userId);
        Task<ErrorOr<Success>> SetEmailConfirmedAsync(string userId);

        // ─── Lockout ─────────────────────────────────────────
        Task<ErrorOr<bool>> IsLockedOutAsync(string userId);
        Task<ErrorOr<Success>> SetLockoutAsync(string userId, DateTimeOffset? endDate);

        // ─── Roles ───────────────────────────────────────────
        Task<ErrorOr<Success>> AddToRoleAsync(string userId, string role);
        Task<ErrorOr<Success>> RemoveFromRoleAsync(string userId, string role);
        Task<ErrorOr<IList<string>>> GetRolesAsync(string userId);

        // ─── RefreshToken ─────────────────────────────────────
        Task<ErrorOr<Success>> AddRefreshTokenAsync(string userId, string token, DateTime expiration);
        Task<ErrorOr<Success>> RevokeRefreshTokenAsync(string userId, string token);

        // ─── Profile ──────────────────────────────────────────
        Task<ErrorOr<Success>> UpdateProfileAsync(string userId, string fullName, string? photoUrl);
        Task<ErrorOr<bool>> IsEmailConfirmedAsync(string userId);
        Task<ErrorOr<bool>> ExistsByEmailAsync(string email);
        Task<ErrorOr<bool>> ExistsByEmailDeletedAsync(string email);
    }
}