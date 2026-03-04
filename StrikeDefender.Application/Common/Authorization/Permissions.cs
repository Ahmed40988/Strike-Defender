using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Authorization
{
public static class Permissions
{
    // Admin
    public const string ManageUsers = "ManageUsers";
    public const string ManagePlans = "ManagePlans";
    public const string ManageSubsciptions = "ManageSubsciptions";

    // Cyber Specialist
    public const string ViewAllAttacks = "ViewAllAttacks";
    public const string ViewAllRules = "ViewAllRules";
    public const string ViewAnalytics = "ViewAnalytics";
    public const string ViewDataset = "ViewDataset";

    // User Features
    public const string LaunchAttack = "LaunchAttack";
    public const string ViewOwnAttacks = "ViewOwnAttacks";
    public const string ViewSubscription = "ViewSubscription";
    public const string UpgradeSubscription = "UpgradeSubscription";
}
}

