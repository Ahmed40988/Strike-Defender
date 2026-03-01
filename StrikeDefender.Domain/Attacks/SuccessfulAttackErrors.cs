using ErrorOr;

namespace StrikeDefender.Domain.Attacks;

public static class SuccessfulAttackErrors
{
    public static Error AttackRequired =>
        Error.Validation("SuccessfulAttack.Attack", "Attack required.");

    public static Error ResultNotSuccessful =>
        Error.Validation("SuccessfulAttack.Invalid", "Attack must be successful first.");
}
