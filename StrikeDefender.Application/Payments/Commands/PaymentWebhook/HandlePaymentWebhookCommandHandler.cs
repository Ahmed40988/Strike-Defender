using ErrorOr;
using MediatR;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Payments.DTO;
using StrikeDefender.Domain.Payments;
using StrikeDefender.Domain.PaymentTransactions;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Subscriptions;

namespace StrikeDefender.Application.Payments.Commands.PaymentWebhook
{
    public class HandlePaymentWebhookCommandHandler
        : IRequestHandler<HandlePaymentWebhookCommand, ErrorOr<Success>>
    {
        private readonly IGenericRepository<Subscription> _subscriptionRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IPaymentTransactionRepository _paymentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymobHmacValidator _hmacValidator;

        public HandlePaymentWebhookCommandHandler(
            IGenericRepository<Subscription> subscriptionRepo,
            IGenericRepository<Plan> planRepo,
           IPaymentTransactionRepository paymentRepo,
            IUserRepository userRepo,
            IUnitOfWork unitOfWork,
            IPaymobHmacValidator hmacValidator)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _paymentRepo = paymentRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _hmacValidator = hmacValidator;
        }

        public async Task<ErrorOr<Success>> Handle(
            HandlePaymentWebhookCommand request,
            CancellationToken cancellationToken)
        {
            var data = request.Data;

            // 🔐 1. Validate HMAC
            if (!_hmacValidator.Validate(data))
                return Error.Unauthorized("Webhook.Invalid", "Invalid HMAC");

            // ❌ 2. Check payment success
            if (!data.Success)
                return Error.Failure("Payment.Failed", "Payment not successful");

            // 🔍 3. Get transaction by OrderId
            var transaction = await _paymentRepo
                .GetByOrderIdAsync(data.OrderId);

            if (transaction is null)
                return Error.NotFound("Transaction.NotFound");

            // 🔁 4. Idempotency (منع التكرار)
            if (transaction.Status == PaymentStatus.Completed)
                return Result.Success;

            // 🧠 5. Get user + plan
            var user = await _userRepo.GetByIdAsync(transaction.UserId);
            if (user is null)
                return Error.NotFound("User.NotFound");

            var plan = await _planRepo.GetByIdAsync(transaction.PlanId);
            if (plan is null)
                return Error.NotFound("Plan.NotFound");

            // 📦 6. Create subscription
            var subResult = Subscription.Create(user.Id, plan);

            if (subResult.IsError)
                return subResult.Errors;

            user.setSubscrition(subResult.Value, user.Id);

            await _subscriptionRepo.AddAsync(subResult.Value, cancellationToken);

            // 🔄 7. Update payment status
            transaction.MarkAsCompleted("system");

            // 💾 8. Save changes
            await _unitOfWork.CommitChangesAsync();

            return Result.Success;
        }
    }
}