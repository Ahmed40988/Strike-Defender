using ErrorOr;

namespace StrikeDefender.Domain.Attacks;

public static class AttackResultErrors
{
    public static Error AttackRequired =>
        Error.Validation("AttackResult.Attack", "Attack is required.");

    public static Error InvalidResponse =>
        Error.Validation("AttackResult.Response", "Response data required.");
}
