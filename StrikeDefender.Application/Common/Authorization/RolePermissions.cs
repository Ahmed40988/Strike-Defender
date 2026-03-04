using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.Common.Authorization
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> Map = new()
        {
            ["Admin"] =
            [
                Permissions.ManageUsers,
            Permissions.ManagePlans,
            Permissions.ViewAllAttacks,
            Permissions.ViewAnalytics,
            Permissions.ViewDataset,
            Permissions.LaunchAttack,
            Permissions.ViewOwnAttacks,
            Permissions.ViewSubscription,
            Permissions.UpgradeSubscription
            ],

            ["CyberSpecialistAdmin"] =
            [
                Permissions.ViewAllAttacks,
            Permissions.ViewAnalytics,
            Permissions.ViewDataset
            ],

            ["User"] =
            [
                Permissions.LaunchAttack,
            Permissions.ViewOwnAttacks,
            Permissions.ViewSubscription,
            Permissions.UpgradeSubscription
            ]
        };
    }
}
