using MusicStreamer.Domain.Enums;

namespace MusicStreamer.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Merchant { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? AuthorizedAt { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? DenialReason { get; private set; }

    // Navigation
    public User? User { get; private set; }
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();

    private Transaction() { }

    public Transaction(Guid userId, string merchant, decimal amount, DateTime requestedAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Merchant = merchant;
        Amount = amount;
        RequestedAt = requestedAt;
        Status = TransactionStatus.Pending;
    }

    public void Authorize()
    {
        Status = TransactionStatus.Authorized;
        AuthorizedAt = DateTime.UtcNow;
    }

    public void Deny(string reason)
    {
        Status = TransactionStatus.Denied;
        DenialReason = reason;
    }
}
