
using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Rules;
using StrikeDefender.Domain.Common.Enums;

namespace StrikeDefender.Domain.IntelligenceDataset;

public class SecurityIntelligenceEntry : BaseModel
{
    public Guid Id { get; private set; }

    public Guid AttackId { get; private set; }
    public Attack Attack { get; private set; } = default!;

    public Guid RuleId { get; private set; }
    public WafRule Rule { get; private set; } = default!;
    public string PayloadSnapshot { get; private set; }
    public string TargetSnapshot { get; private set; }
    public string RuleSnapshot { get; private set; }
    public AttackType AttackType { get; private set; }// SQLi, XSS,  etc.
    public SeverityLevel Severity { get; private set; }// Low,Medium,High
    public string AiAnalysis { get; private set; } = string.Empty;
    public string SecurityAnalysis { get; private set; } = string.Empty;

    public int RiskScore { get; private set; }

    private SecurityIntelligenceEntry() { }

    public static ErrorOr<SecurityIntelligenceEntry> Create(
        Attack attack,
        WafRule rule,
        AttackType attackType,
        SeverityLevel severity,
        AttackCategory category,
        string aiAnalysis,
        string securityAnalysis,
        int riskScore)

    {
        if (attack is null)
            return SecurityIntelligenceEntryErrors.AttackRequired;

        if (rule is null)
            return SecurityIntelligenceEntryErrors.RuleRequired;

        if (string.IsNullOrWhiteSpace(aiAnalysis))
            return SecurityIntelligenceEntryErrors.AiAnalysisRequired;

        if (string.IsNullOrWhiteSpace(securityAnalysis))
            return SecurityIntelligenceEntryErrors.SecurityAnalysisRequired;

        return new SecurityIntelligenceEntry
        {
            Id = Guid.NewGuid(),

            AttackId = attack.Id,
            Attack = attack,

            RuleId = rule.Id,
            Rule = rule,

            AttackType = attackType,
            Severity = severity,

            AiAnalysis = aiAnalysis,
            SecurityAnalysis = securityAnalysis,
            RiskScore = riskScore
        };
    }

    public ErrorOr<Success> UpdateAnalysis(
        string aiAnalysis,
        string securityAnalysis,
        int riskScore,
        string updatedById)
    {
        if (string.IsNullOrWhiteSpace(aiAnalysis))
            return SecurityIntelligenceEntryErrors.AiAnalysisRequired;

        if (string.IsNullOrWhiteSpace(securityAnalysis))
            return SecurityIntelligenceEntryErrors.SecurityAnalysisRequired;

        AiAnalysis = aiAnalysis;
        SecurityAnalysis = securityAnalysis;
        RiskScore = riskScore;

        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteEntry(string updatedById)
    {
        SoftDelete(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> RestoreEntry(string updatedById)
    {
        Restore(updatedById);
        return Result.Success;
    }
}
