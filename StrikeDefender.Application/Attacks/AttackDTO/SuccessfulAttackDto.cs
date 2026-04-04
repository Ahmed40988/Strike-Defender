namespace StrikeDefender.Application.Attacks.AttackDTO
{
    public record SuccessfulAttackDto
    (
        Guid AttackId,
        string Payload,
        string Target,
        string Technique,
        string Severity,
        bool IsBlockedByWaf,
        int StatusCode,
        string Result,
        double ExecutionTimeMs
    );
}
