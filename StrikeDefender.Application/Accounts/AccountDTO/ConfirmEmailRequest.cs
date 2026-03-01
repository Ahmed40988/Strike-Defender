namespace StrikeDefender.Application.Accounts.AccountDTO;

public record ConfirmEmailRequest(
    string email,
    string OTP
);