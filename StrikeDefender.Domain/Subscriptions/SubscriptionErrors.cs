using ErrorOr;

namespace StrikeDefender.Domain.Subscriptions;

public static class SubscriptionErrors
{
    public static Error UserIdRequired =>
        Error.Validation("Subscription.User.Required", "UserId is required.");

    public static Error PlanIdRequired =>
        Error.Validation("Subscription.Plan.Required", "PlanId is required.");

    public static Error InvalidDates =>
        Error.Validation("Subscription.Dates.Invalid", "End date must be after start date.");

    public static Error AlreadyExpired =>
        Error.Validation("Subscription.Expired", "Subscription already expired.");

    public static Error AlreadyActive =>
        Error.Validation("Subscription.Active", "Subscription already active.");

    public static Error RenewalFailed =>
        Error.Validation("Subscription.Renewal.Failed", "Failed to renew subscription.");

    public static Error PlanRequired = 
        Error.Validation("Subscription.Plan.Required", "Plan is required for subscription.");

    public static Error NotFound => Error.NotFound(
     code: "Subscription.NotFound",
     description: "No subscription found for this user");

    public static Error Expired => Error.Unauthorized(
        code: "Subscription.Expired",
        description: "Your subscription has expired");

    public static Error Inactive => Error.Unauthorized(
        code: "Subscription.Inactive",
        description: "Your subscription is inactive");

    public static Error NoAccess => Error.Forbidden(
        code: "Subscription.NoAccess",
        description: "You are not allowed to access this dataset");

}
