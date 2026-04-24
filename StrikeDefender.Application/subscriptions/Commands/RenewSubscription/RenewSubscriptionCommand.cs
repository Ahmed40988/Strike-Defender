using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.RenewSubscription
{
    public record RenewSubscriptionCommand(string UserId)
     : IRequest<ErrorOr<string>>;
}
