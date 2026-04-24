using StrikeDefender.Application.Common.Interfaces;
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
        IPaymentService paymentService,
            IGenericRepository<Plan> planRepo,
              IUserRepository userRepository,
            ISubscriptionAccessService subscriptionRepo,
            IGenericRepository<Subscription> subscriptionGenericRepo,
            IUnitOfWork unitOfWork)
        : IRequestHandler<SubscribeToPlanCommand, ErrorOr<string>>
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IGenericRepository<Plan> _planRepo = planRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
        private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = subscriptionGenericRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task<ErrorOr<string>> Handle(
           SubscribeToPlanCommand request,
           CancellationToken cancellationToken)
        {
            var existing = await _subscriptionRepo
                .HasActiveSubscriptionAsync(request.UserId);

            if (existing)
                return Error.Conflict("Subscription.Exists",
                    "User already has an active subscription");

            var plan = await _planRepo.GetByIdAsync(request.PlanId);
            if (plan is null)
                return Error.NotFound("Plan.NotFound");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Error.NotFound("User.NotFound");

            var paymentUrl = await _paymentService.ProcessPaymentAsync(user, plan);

            return paymentUrl;
        }
    }
}


