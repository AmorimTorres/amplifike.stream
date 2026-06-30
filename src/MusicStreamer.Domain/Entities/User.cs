using MusicStreamer.Domain.Enums;

namespace MusicStreamer.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "User";
    public AccountStatus AccountStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public Guid? SubscriptionPlanId { get; private set; }
    public SubscriptionPlan? SubscriptionPlan { get; private set; }
    public ICollection<UserSubscription> Subscriptions { get; private set; } = new List<UserSubscription>();
    public ICollection<Playlist> Playlists { get; private set; } = new List<Playlist>();
    public ICollection<FavoriteMusic> FavoriteMusics { get; private set; } = new List<FavoriteMusic>();
    public ICollection<FavoriteBand> FavoriteBands { get; private set; } = new List<FavoriteBand>();
    public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private User() { }

    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        AccountStatus = AccountStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string name)
    {
        Name = name;
    }

    public void SetRole(string role)
    {
        Role = role;
    }

    public void Activate() => AccountStatus = AccountStatus.Active;
    public void Suspend() => AccountStatus = AccountStatus.Suspended;
    public void Block() => AccountStatus = AccountStatus.Blocked;

    public bool IsActive() => AccountStatus == AccountStatus.Active;
}
