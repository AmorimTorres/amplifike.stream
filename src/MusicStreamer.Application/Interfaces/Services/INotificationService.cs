using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Application.Interfaces.Services;

public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string userEmail, Guid transactionId, string message, CancellationToken cancellationToken = default);
    Task SendToMerchantAsync(string merchantEmail, Guid transactionId, string message, CancellationToken cancellationToken = default);
}
