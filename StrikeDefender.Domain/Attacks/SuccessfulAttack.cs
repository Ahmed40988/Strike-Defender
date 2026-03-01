using ErrorOr;
using StrikeDefender.Domain.BaseModels;

namespace StrikeDefender.Domain.Attacks;

public class SuccessfulAttack : BaseModel
{
    public Guid Id { get; private set; }

    public Guid AttackId { get; private set; }
    public Attack Attack { get; private set; } = default!;

    public Guid AttackResultId { get; private set; }
    public AttackResult AttackResult { get; private set; } = default!;

    public string BypassTechnique { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    private SuccessfulAttack() { }

    public static ErrorOr<SuccessfulAttack> Create(
        Attack attack,
        AttackResult result,
        string bypassTechnique,
        string notes)
    {
        if (attack is null)
            return SuccessfulAttackErrors.AttackRequired;

        if (result is null || !result.IsSuccessful)
            return SuccessfulAttackErrors.ResultNotSuccessful;

        return new SuccessfulAttack
        {
            Id = Guid.NewGuid(),
            AttackId = attack.Id,
            Attack = attack,
            AttackResultId = result.Id,
            AttackResult = result,
            BypassTechnique = bypassTechnique ?? string.Empty,
            Notes = notes ?? string.Empty
        };
    }

    public ErrorOr<Success> UpdateNotes(string notes, string updatedById)
    {
        Notes = notes ?? string.Empty;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteSuccessfulAttack(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestoreSuccessfulAttack(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }
}
