using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Common.Enums;
using StrikeDefender.Domain.Rules;

namespace StrikeDefender.Domain.Attacks;

public class Attack : BaseModel
{
    public Guid Id { get; private set; }

    public string Payload { get; private set; } = string.Empty;

    public AttackType Type { get; private set; }

    public bool IsSuccessful { get; private set; }

    public Guid ?RuleId { get; private set; }
    public WafRule Rule { get; private set; } = default!;


    private Attack() { }

    public static ErrorOr<Attack> Create(
        string payload,
        AttackType type)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return AttackErrors.PayloadRequired;

        return new Attack
        {
            Id = Guid.NewGuid(),
            Payload = payload,
            Type = type,
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
        string updatedById)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return AttackErrors.PayloadRequired;

        Payload = payload;

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

    public ErrorOr<Success> AssociateWithRule(Guid  ruleId, string updatedById)
    {

            RuleId = ruleId;
            Touch(updatedById);
            return Result.Success;
    }
}
