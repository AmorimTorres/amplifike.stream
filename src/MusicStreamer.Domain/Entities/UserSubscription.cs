namespace MusicStreamer.Domain.Entities;

public class UserSubscription
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User? User { get; private set; }
    public SubscriptionPlan? SubscriptionPlan { get; private set; }

    private UserSubscription() { }

    public UserSubscription(Guid userId, Guid subscriptionPlanId, int durationInDays)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        SubscriptionPlanId = subscriptionPlanId;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddDays(durationInDays);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public bool HasExpired() => DateTime.UtcNow > EndDate;

    public void Deactivate() => IsActive = false;
}
