using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StrikeDefender.Application.IntelligenceDatasets.IntelligenceDatasetsDTO
{
    public record SecurityEntryResponse
    (
        Guid Id,
        string PayloadSnapshot,
        string TargetSnapshot,
        string RuleSnapshot,
        string AttackType,
        string Severity,
        int RiskScore,
        string AiAnalysis,
        string SecurityAnalysis
    );
}
