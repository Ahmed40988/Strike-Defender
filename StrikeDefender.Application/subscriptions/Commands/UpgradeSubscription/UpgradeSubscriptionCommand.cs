using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.UpgradeSubscription
{
    public record UpgradeSubscriptionCommand(Guid NewPlanId, string UserId)
        : IRequest<ErrorOr<Success>>;
}
