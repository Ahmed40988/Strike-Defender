using ErrorOr;
using StrikeDefender.Domain.BaseModels;

namespace StrikeDefender.Domain.Attacks;

public class AttackResult : BaseModel
{
    public Guid Id { get; private set; }

    public Guid AttackId { get; private set; }
    public Attack Attack { get; private set; } = default!;

    public bool IsBlockedByWaf { get; private set; }
    public bool IsExecuted { get; private set; }
    public bool IsSuccessful { get; private set; }

    public int ResponseCode { get; private set; }
    public string ResponseBody { get; private set; } = string.Empty;

    public double ExecutionTimeMs { get; private set; }

    private AttackResult() { }

    public static ErrorOr<AttackResult> Create(
        Attack attack,
        bool isBlockedByWaf,
        bool isExecuted,
        bool isSuccessful,
        int responseCode,
        string responseBody,
        double executionTimeMs)
    {
        if (attack is null)
            return AttackResultErrors.AttackRequired;

        return new AttackResult
        {
            Id = Guid.NewGuid(),
            AttackId = attack.Id,
            Attack = attack,
            IsBlockedByWaf = isBlockedByWaf,
            IsExecuted = isExecuted,
            IsSuccessful = isSuccessful,
            ResponseCode = responseCode,
            ResponseBody = responseBody ?? string.Empty,
            ExecutionTimeMs = executionTimeMs
        };
    }

    public ErrorOr<Success> MarkSuccessful(string updatedById)
    {
        IsSuccessful = true;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> MarkBlocked(string updatedById)
    {
        IsBlockedByWaf = true;
        IsSuccessful = false;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteResult(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestoreResult(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }
}
