namespace MusicStreamer.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int DurationInDays { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation
    public ICollection<UserSubscription> Subscriptions { get; private set; } = new List<UserSubscription>();

    private SubscriptionPlan() { }

    public SubscriptionPlan(string name, decimal price, int durationInDays)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        DurationInDays = durationInDays;
        IsActive = true;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
