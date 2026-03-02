using StrikeDefender.Domain.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.RenewSubscription
{
    public class RenewSubscriptionCommandHandler(ISubscriptionAccessService subscriptionRepo
        , IUnitOfWork unitOfWork
        , IGenericRepository<Subscription> SubscriptiongenericRepository)
        : IRequestHandler<RenewSubscriptionCommand, ErrorOr<Success>>
    {
        private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
        private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = SubscriptiongenericRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ErrorOr<Success>> Handle(
            RenewSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var sub = await _subscriptionRepo
                .FirstOrDefaultByUserIdAsync(request.UserId);

            if (sub is null)
                return SubscriptionErrors.NotFound;

            var plan = sub.Plan;

            var renewResult = sub.Renew(plan, request.UserId);

            if (renewResult.IsError)
                return renewResult.Errors;

            await _subscriptionGenericRepo.UpdateAsync(sub, cancellationToken);
            await _unitOfWork.CommitChangesAsync();

            return Result.Success;
        }
    }
}
