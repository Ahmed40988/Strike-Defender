using StrikeDefender.Application.subscriptions.Commands.UpgradeSubscription;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;

public class UpgradeSubscriptionCommandHandler(
    IGenericRepository<Plan> planRepo,
    ISubscriptionAccessService subscriptionRepo,
    IUserRepository userRepository,
    IPaymentService paymentService)
    : IRequestHandler<UpgradeSubscriptionCommand, ErrorOr<string>>
{
    private readonly IGenericRepository<Plan> _planRepo = planRepo;
    private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<string>> Handle(
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

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            return Error.NotFound("User.NotFound");

        var paymentUrl = await _paymentService.ProcessPaymentAsync(user, newPlan);

        return paymentUrl;
    }
}