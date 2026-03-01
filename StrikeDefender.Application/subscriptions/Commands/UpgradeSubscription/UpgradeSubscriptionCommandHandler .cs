using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.UpgradeSubscription
{
    public class UpgradeSubscriptionCommandHandler(IGenericRepository<Plan> planRepo,
   ISubscriptionAccessService subscriptionRepo,
   IUserRepository userRepository,
   IGenericRepository<Subscription> subscriptionGenericRepo,
   IUnitOfWork unitOfWork) : IRequestHandler<UpgradeSubscriptionCommand, ErrorOr<Success>>
    {
        private readonly IGenericRepository<Plan> _planRepo = planRepo;
        private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = subscriptionGenericRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ErrorOr<Success>> Handle(
            UpgradeSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _subscriptionRepo
                .FirstOrDefaultByUserIdAsync(request.UserId);

            if (existing is null)
                return SubscriptionErrors.NotFound;

            var newPlan = await _planRepo.GetByIdAsync(request.NewPlanId);

            if (newPlan is null)
                return Error.NotFound("Plan.Notfound", "Plan With This Id Not Found");

            var newSub = Subscription.Create(request.UserId, newPlan);
            if (newSub.IsError)
                return newSub.Errors;

            await _subscriptionGenericRepo.AddAsync(newSub.Value, cancellationToken);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            user.UpdateSubscription(newSub.Value, request.UserId);

            existing.Deactivate(request.UserId);
            await _subscriptionGenericRepo.UpdateAsync(existing, cancellationToken);

            await _unitOfWork.CommitChangesAsync();

            return Result.Success;
        }
    }
}
