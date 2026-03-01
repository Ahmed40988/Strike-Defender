namespace StrikeDefender.Application.Accounts.AccountDTO;

public record RefreshTokenRequest(
    string Token,
    string RefreshToken
);