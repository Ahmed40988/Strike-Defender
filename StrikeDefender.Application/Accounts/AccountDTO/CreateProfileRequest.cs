
namespace StrikeDefender.Application.Accounts.AccountDTO
{
    public record CreateProfileRequest
        (
        string FullName,
        string Phone,
        DateOnly DateOfBirth,
        IFormFile Image
        );
}
