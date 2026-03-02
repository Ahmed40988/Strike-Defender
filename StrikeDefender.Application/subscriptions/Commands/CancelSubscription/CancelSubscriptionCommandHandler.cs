using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.CancelSubscription
{
    using ErrorOr;
    using MediatR;
    using StrikeDefender.Application.Common.Interfaces;
    using StrikeDefender.Domain.Subscriptions;

    public class CancelSubscriptionCommandHandler(ISubscriptionAccessService subscriptionRepo
        , IUnitOfWork unitOfWork
        ,IGenericRepository<Subscription> SubscriptiongenericRepository)
        : IRequestHandler<CancelSubscriptionCommand, ErrorOr<Success>>
    {
        private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
        private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = SubscriptiongenericRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ErrorOr<Success>> Handle(
            CancelSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var sub = await _subscriptionRepo
                .FirstOrDefaultByUserIdAsync(request.UserId);

            if (sub is null)
                return SubscriptionErrors.NotFound;

            sub.Deactivate(request.UserId);

            await _subscriptionGenericRepo.UpdateAsync(sub, cancellationToken);
            await _unitOfWork.CommitChangesAsync();

            return Result.Success;
        }
    }
}
