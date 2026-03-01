using ErrorOr;

namespace StrikeDefender.Domain.IntelligenceDataset;

public static class SecurityIntelligenceEntryErrors
{
    public static Error AttackRequired =>
        Error.Validation("Dataset.Attack", "Attack is required.");

    public static Error RuleRequired =>
        Error.Validation("Dataset.Rule", "Rule is required.");

    public static Error AiAnalysisRequired =>
        Error.Validation("Dataset.AI", "AI analysis required.");

    public static Error SecurityAnalysisRequired =>
        Error.Validation("Dataset.Security", "Security analysis required.");
    public static Error NoData => Error.NotFound(
        code: "Dataset.Empty",
        description: "No intelligence data available for your plan");

    public static Error InvalidUser => Error.Unauthorized(
        code: "Auth.InvalidUser",
        description: "Invalid user session");
}

