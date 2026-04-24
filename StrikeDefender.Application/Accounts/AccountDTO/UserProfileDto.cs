namespace StrikeDefender.Application.Accounts.AccountDTO
{
    public record UserProfileDto(
       string Id,
       string Email,
       string FullName,
       string? PhotoUrl,
       DateOnly DateOfBirth,
       DateTime CreatedOn
   );
}
