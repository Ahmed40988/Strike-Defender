using ErrorOr;
using StrikeDefender.Domain.BaseModels;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Users;

namespace StrikeDefender.Domain.Subscriptions 
{ 
public class Subscription : BaseModel
{
    public Guid Id { get; private set; }

    public string UserId { get; private set; } = string.Empty;
    public AppUser User { get; private set; } = default!;

    public Guid PlanId { get; private set; }
    public Plan Plan { get; private set; } = default!;

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public bool IsActive { get; private set; }

    private Subscription() { }

    public static ErrorOr<Subscription> Create(string userId, Plan plan)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return SubscriptionErrors.UserIdRequired;

        if (plan is null)
            return SubscriptionErrors.PlanRequired;

        var start = DateTime.UtcNow;
        var end = start.AddDays(plan.DurationInDays);

        return new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = plan.Id,
            Plan = plan,
            StartDate = start,
            EndDate = end,
            IsActive = true
        };
    }

    public bool IsExpired()
        => DateTime.UtcNow > EndDate;

    public bool HasAccess()
        => IsActive && !IsExpired();

    public ErrorOr<Success> Renew(Plan plan, string updatedById)
    {
        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddDays(plan.DurationInDays);
        IsActive = true;

        Touch(updatedById);
        return Result.Success;
    }

    public ErrorOr<Success> Deactivate(string updatedById)
    {
        IsActive = false;
        Touch(updatedById);
        return Result.Success;
    }
}
}