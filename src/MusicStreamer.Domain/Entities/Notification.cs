using MusicStreamer.Domain.Enums;

namespace MusicStreamer.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid TransactionId { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; }
    public NotificationStatus Status { get; private set; }

    // Navigation
    public Transaction? Transaction { get; private set; }

    private Notification() { }

    public Notification(Guid transactionId, string recipient, string message)
    {
        Id = Guid.NewGuid();
        TransactionId = transactionId;
        Recipient = recipient;
        Message = message;
        SentAt = DateTime.UtcNow;
        Status = NotificationStatus.Pending;
    }

    public void MarkSent() => Status = NotificationStatus.Sent;
    public void MarkFailed() => Status = NotificationStatus.Failed;
}
