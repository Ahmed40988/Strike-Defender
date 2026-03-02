using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.CancelSubscription
{
    public record CancelSubscriptionCommand(string UserId)
        : IRequest<ErrorOr<Success>>;
}
