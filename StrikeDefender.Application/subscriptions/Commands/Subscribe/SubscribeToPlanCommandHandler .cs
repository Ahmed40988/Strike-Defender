using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Application.subscriptions.Commands.Subscribe
{
    public class SubscribeToPlanCommandHandler(
            IGenericRepository<Plan> planRepo,
              IUserRepository userRepository,
            ISubscriptionAccessService subscriptionRepo,
            IGenericRepository<Subscription> subscriptionGenericRepo,
            IUnitOfWork unitOfWork)
        : IRequestHandler<SubscribeToPlanCommand, ErrorOr<Success>>
    {
        private readonly IGenericRepository<Plan> _planRepo = planRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
        private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = subscriptionGenericRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<ErrorOr<Success>> Handle(
            SubscribeToPlanCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _subscriptionRepo
                .HasActiveSubscriptionAsync(request.UserId);

            if (existing)
                return Error.Conflict("Subscription.Exists",
                    "User already has an active subscription Use Upgrade Plan");

            var plan = await _planRepo.GetByIdAsync(request.PlanId);

            if (plan is null)
                return Error.NotFound("Plan.Notfound","Plan With This Id Not Found");

            var subResult = Subscription.Create(request.UserId, plan);
            var user = await _userRepository.GetByIdAsync(request.UserId);
             user.setSubscrition(subResult.Value,request.UserId);

            if (subResult.IsError)
                return subResult.Errors;

            await _subscriptionGenericRepo.AddAsync(subResult.Value, cancellationToken);
            await _unitOfWork.CommitChangesAsync();

            return Result.Success;
        }
    }
}
