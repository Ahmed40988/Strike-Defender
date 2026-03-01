using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Subscriptions;

namespace StrikeDefender.Domain.Plans;

public class Plan : BaseModel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    public int MaxAttacks { get; private set; }
    public int MaxRules { get; private set; }
    public int DurationInDays { get; private set; }

    // 🔥  التحكم في الداتا اللي يشوفها المشترك ف الخطه دي
    public int MaxRiskScoreAccess { get; private set; }

    private readonly List<Subscription> _subscriptions = new();
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

    private Plan() { }

    public static ErrorOr<Plan> Create(
        string name,
        decimal price,
        int maxAttacks,
        int maxRules,
        int durationInDays,
        int maxRiskScoreAccess)
    {
        if (string.IsNullOrWhiteSpace(name))
            return PlanErrors.NameIsRequired;

        if (price < 0)
            return PlanErrors.InvalidPrice;

        if (maxAttacks < 0 || maxRules < 0)
            return PlanErrors.InvalidLimits;

        if (durationInDays <= 0)
            return PlanErrors.InvalidDuration;

        if (maxRiskScoreAccess < 0 || maxRiskScoreAccess > 100)
            return PlanErrors.InvalidRiskAccess;

        return new Plan
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Price = price,
            MaxAttacks = maxAttacks,
            MaxRules = maxRules,
            DurationInDays = durationInDays,
            MaxRiskScoreAccess = maxRiskScoreAccess
        };
    }

    public ErrorOr<Success> Update(
        string name,
        decimal price,
        int maxAttacks,
        int maxRules,
        int durationInDays,
        int maxRiskScoreAccess,
        string updatedById)
    {
        if (string.IsNullOrWhiteSpace(name))
            return PlanErrors.NameIsRequired;

        if (price < 0)
            return PlanErrors.InvalidPrice;

        if (maxAttacks < 0 || maxRules < 0)
            return PlanErrors.InvalidLimits;

        if (durationInDays <= 0)
            return PlanErrors.InvalidDuration;

        if (maxRiskScoreAccess < 0 || maxRiskScoreAccess > 100)
            return PlanErrors.InvalidRiskAccess;

        Name = name.Trim();
        Price = price;
        MaxAttacks = maxAttacks;
        MaxRules = maxRules;
        DurationInDays = durationInDays;
        MaxRiskScoreAccess = maxRiskScoreAccess;

        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeletePlan(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestorePlan(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }
}