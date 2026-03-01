using ErrorOr;

namespace StrikeDefender.Domain.Rules;

public static class RuleErrors
{
    public static Error ContentRequired =>
        Error.Validation("Rule.Content", "Rule content is required.");

    public static Error AttackRequired =>
        Error.Validation("Rule.Attack", "Attack reference required.");
}
