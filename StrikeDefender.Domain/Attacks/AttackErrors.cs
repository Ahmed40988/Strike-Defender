using ErrorOr;

namespace StrikeDefender.Domain.Attacks;

public static class AttackErrors
{
    public static Error PayloadRequired =>
        Error.Validation("Attack.Payload", "Payload is required.");
    public static Error RuleRequired =>
        Error.Validation("Rule.Required", "Rule is required.");

    public static Error TargetRequired =>
        Error.Validation("Attack.Target", "Target is required.");

    public static Error InvalidSeverity =>
        Error.Validation("Attack.Severity", "Severity is required.");
}
