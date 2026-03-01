using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Common.Enums;
using StrikeDefender.Domain.Rules;

namespace StrikeDefender.Domain.Attacks;

public class Attack : BaseModel
{
    public Guid Id { get; private set; }

    public string Payload { get; private set; } = string.Empty;
    public string Target { get; private set; } = string.Empty;

    public AttackType Type { get; private set; }
    public SeverityLevel Severity { get; private set; }

    public bool IsSuccessful { get; private set; }

    public Guid RuleId { get; private set; }
    public WafRule Rule { get; private set; } = default!;


    private Attack() { }

    public static ErrorOr<Attack> Create(
        string payload,
        string target,
        AttackType type,
        SeverityLevel severity)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return AttackErrors.PayloadRequired;

        if (string.IsNullOrWhiteSpace(target))
            return AttackErrors.TargetRequired;

        return new Attack
        {
            Id = Guid.NewGuid(),
            Payload = payload,
            Target = target,
            Type = type,
            Severity = severity,
            IsSuccessful = false
        };
    }

    public ErrorOr<Success> MarkAsSuccessful(string updatedById)
    {
        IsSuccessful = true;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> MarkAsFailed(string updatedById)
    {
        IsSuccessful = false;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> UpdatePayload(
        string payload,
        SeverityLevel severity,
        string updatedById)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return AttackErrors.PayloadRequired;

        Payload = payload;
        Severity = severity;

        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteAttack(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestoreAttack(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> AssociateWithRule(WafRule rule, string updatedById)
    {
        if (rule is null)
            return AttackErrors.RuleRequired;
            RuleId = rule.Id;
            Rule = rule;
            Touch(updatedById);
            return Result.Success;
    }
}
