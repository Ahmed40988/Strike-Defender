using StrikeDefender.Application.subscriptions.Commands.RenewSubscription;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;

public class RenewSubscriptionCommandHandler(
    ISubscriptionAccessService subscriptionRepo,
    IGenericRepository<Subscription> subscriptionGenericRepository,
    IGenericRepository<Plan> planRepo,
    IUserRepository userRepo,
    IPaymentService paymentService)
    : IRequestHandler<RenewSubscriptionCommand, ErrorOr<string>>
{
    private readonly ISubscriptionAccessService _subscriptionRepo = subscriptionRepo;
    private readonly IGenericRepository<Subscription> _subscriptionGenericRepo = subscriptionGenericRepository;
    private readonly IGenericRepository<Plan> _planRepo = planRepo;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<string>> Handle(
        RenewSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var sub = await _subscriptionRepo
            .FirstOrDefaultByUserIdAsync(request.UserId);

        if (sub is null)
            return SubscriptionErrors.NotFound;

        var user = await _userRepo.GetByIdAsync(request.UserId);
        if (user is null)
            return Error.NotFound("User.NotFound");

        var plan = await _planRepo.GetByIdAsync(sub.PlanId);
        if (plan is null)
            return Error.NotFound("Plan.NotFound");

        var paymentUrl = await _paymentService.ProcessPaymentAsync(user, plan);

        return paymentUrl;
    }
}