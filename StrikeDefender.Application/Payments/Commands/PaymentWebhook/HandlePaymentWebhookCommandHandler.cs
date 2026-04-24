using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IGenericRepository<PaymentTransaction> _GenpaymentRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IPaymentTransactionRepository _paymentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HandlePaymentWebhookCommandHandler> _logger;


        public HandlePaymentWebhookCommandHandler(
            IGenericRepository<Subscription> subscriptionRepo,
            IGenericRepository<Plan> planRepo,
           IPaymentTransactionRepository paymentRepo,
            IUserRepository userRepo,
            IUnitOfWork unitOfWork,
            ILogger<HandlePaymentWebhookCommandHandler> logger,
            IGenericRepository<PaymentTransaction> genpaymentRepo)
        {
            _subscriptionRepo = subscriptionRepo;
            _planRepo = planRepo;
            _paymentRepo = paymentRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _GenpaymentRepo = genpaymentRepo;
        }

        public async Task<ErrorOr<Success>> Handle(
            HandlePaymentWebhookCommand request,
            CancellationToken cancellationToken)
        {
            var data = request.Data.Obj;

            var orderId = data.Order.Id;
            var success = data.Success;

            _logger.LogInformation("Webhook received | OrderId: {OrderId} | Success: {Success}", orderId, success);

            var transaction = await _paymentRepo.GetByOrderIdAsync(orderId);

            if (transaction is null)
            {
                _logger.LogError("Transaction NOT FOUND | OrderId: {OrderId}", orderId);
                return Result.Success;
            }

            if (transaction.Status == PaymentStatus.Completed)
            {
                _logger.LogInformation("Duplicate webhook ignored | OrderId: {OrderId}", orderId);
                return Result.Success;
            }

            if (!success)
            {
                _logger.LogWarning("Payment FAILED | OrderId: {OrderId}", orderId);

                transaction.MarkAsFailed("paymob");
                await _unitOfWork.CommitChangesAsync();

                return Result.Success;
            }

            var user = await _userRepo.GetByIdAsync(transaction.UserId);
            if (user is null)
            {
                _logger.LogError("User NOT FOUND | UserId: {UserId}", transaction.UserId);
                return Result.Success;
            }

            var plan = await _planRepo.GetByIdAsync(transaction.PlanId);
            if (plan is null)
            {
                _logger.LogError("Plan NOT FOUND | PlanId: {PlanId}", transaction.PlanId);
                return Result.Success;
            }

            var subResult = Subscription.Create(user.Id, plan);

            if (subResult.IsError)
            {
                _logger.LogError("Subscription creation failed | UserId: {UserId}", user.Id);
                return Result.Success;
            }

            user.setSubscrition(subResult.Value, user.Id);
           await _userRepo.UpdateAsync(user, cancellationToken);

            await _subscriptionRepo.AddAsync(subResult.Value, cancellationToken);

            transaction.MarkAsCompleted("paymob");

            await _GenpaymentRepo.UpdateAsync(transaction, cancellationToken);
            await _unitOfWork.CommitChangesAsync();

            _logger.LogInformation("Webhook processing COMPLETED SUCCESSFULLY | OrderId: {OrderId} | UserId: {UserId}", orderId, user.Id);

            return Result.Success;
        }
    }
    }