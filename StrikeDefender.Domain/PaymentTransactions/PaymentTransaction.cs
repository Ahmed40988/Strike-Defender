using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.PaymentTransactions;

namespace StrikeDefender.Domain.Payments;

public class PaymentTransaction : BaseModel
{
    public Guid Id { get; private set; }

    public int OrderId { get; private set; } // Paymob OrderId

    public string UserId { get; private set; }
    public Guid PlanId { get; private set; }

    public decimal Amount { get; private set; }

    public PaymentStatus Status { get; private set; }

    private PaymentTransaction() { }

    
    public static ErrorOr<PaymentTransaction> Create(
        int orderId,
        string userId,
        Guid planId,
        decimal amount)
    {
        if (orderId <= 0)
            return PaymentErrors.InvalidOrderId;

        if (userId == string.Empty)
            return PaymentErrors.InvalidUser;

        if (planId == Guid.Empty)
            return PaymentErrors.InvalidPlan;

        if (amount <= 0)
            return PaymentErrors.InvalidAmount;

        return new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            UserId = userId,
            PlanId = planId,
            Amount = amount,
            Status = PaymentStatus.Pending
        };
    }

 
    public ErrorOr<Success> MarkAsCompleted(string updatedBy)
    {
        if (Status == PaymentStatus.Completed)
            return PaymentErrors.AlreadyCompleted;

        Status = PaymentStatus.Completed;

        Touch(updatedBy);
        return Result.Success;
    }

    
    public ErrorOr<Success> MarkAsFailed(string updatedBy)
    {
        if (Status == PaymentStatus.Failed)
            return PaymentErrors.AlreadyFailed;

        Status = PaymentStatus.Failed;

        Touch(updatedBy);
        return Result.Success;
    }
}