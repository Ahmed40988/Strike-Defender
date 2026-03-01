using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.Subscribe
{
    public record SubscribeToPlanCommand(Guid PlanId, string UserId)
        : IRequest<ErrorOr<Success>>;
}
