using ErrorOr;

namespace StrikeDefender.Domain.Plans;

public static class PlanErrors
{
    public static Error NameIsRequired =>
        Error.Validation("Plan.Name.Required", "Plan name is required.");

    public static Error InvalidPrice =>
        Error.Validation("Plan.Price.Invalid", "Price must be greater than or equal to zero.");

    public static Error InvalidLimits =>
        Error.Validation("Plan.Limits.Invalid", "Limits must be positive values.");

    public static Error InvalidDuration =>
        Error.Validation("Plan.Duration.Invalid", "Duration must be greater than zero.");

    public static Error InvalidRiskAccess =>
        Error.Validation("Plan.RiskAccess.Invalid", "Risk access must be between 0 and 100.");
}
