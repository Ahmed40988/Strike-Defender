using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Common.Enums;

namespace StrikeDefender.Domain.Rules;

public class WafRule : BaseModel
{
    public Guid Id { get; private set; }

    public string RuleContent { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private readonly List<Attack> _attacks = new();
    public IReadOnlyCollection<Attack> Attacks => _attacks.AsReadOnly();

    public RuleStatus Status { get; private set; }

    public int Version { get; private set; }

    private WafRule() { }

    public static ErrorOr<WafRule> Create(
        string ruleContent,
        string description)
    {

        if (string.IsNullOrWhiteSpace(ruleContent))
            return RuleErrors.ContentRequired;

        return new WafRule
        {
            Id = Guid.NewGuid(),
            RuleContent = ruleContent,
            Description = description ?? string.Empty,
            Status = RuleStatus.Generated,
            Version = 1
        };
    }

    public ErrorOr<Success> Activate(string updatedById)
    {
        Status = RuleStatus.Active;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> Disable(string updatedById)
    {
        Status = RuleStatus.Disabled;
        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateRule(
        string newContent,
        string description,
        string updatedById)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            return RuleErrors.ContentRequired;

        RuleContent = newContent;
        Description = description ?? string.Empty;
        Version++;

        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteRule(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestoreRule(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }
}
