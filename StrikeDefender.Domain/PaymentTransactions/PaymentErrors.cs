using ErrorOr;

namespace StrikeDefender.Domain.Payments;

public static class PaymentErrors
{
    public static Error InvalidOrderId =>
        Error.Validation("Payment.InvalidOrderId", "OrderId is invalid");

    public static Error InvalidUser =>
        Error.Validation("Payment.InvalidUser", "UserId is invalid");

    public static Error InvalidPlan =>
        Error.Validation("Payment.InvalidPlan", "PlanId is invalid");

    public static Error InvalidAmount =>
        Error.Validation("Payment.InvalidAmount", "Amount must be greater than zero");

    public static Error AlreadyCompleted =>
        Error.Conflict("Payment.AlreadyCompleted", "Payment already completed");

    public static Error AlreadyFailed =>
        Error.Conflict("Payment.AlreadyFailed", "Payment already failed");
}