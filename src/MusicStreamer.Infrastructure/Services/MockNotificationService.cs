using Microsoft.Extensions.Logging;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Infrastructure.Services;

/// <summary>
/// Implementação simulada do serviço de notificação.
/// Em produção, seria substituída por integrações reais (e-mail, SMS, push, etc.).
/// </summary>
public class MockNotificationService : INotificationService
{
    private readonly ILogger<MockNotificationService> _logger;
    private readonly ITransactionRepository _transactionRepository;

    public MockNotificationService(
        ILogger<MockNotificationService> logger,
        ITransactionRepository transactionRepository)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
    }

    public async Task SendToUserAsync(
        Guid userId,
        string userEmail,
        Guid transactionId,
        string message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[NOTIFICAÇÃO] Para usuário {Email}: {Message} (TransactionId: {TransactionId})",
            userEmail, message, transactionId);

        var notification = new Notification(transactionId, userEmail, message);
        notification.MarkSent();

        await _transactionRepository.AddNotificationAsync(notification, cancellationToken);
    }

    public async Task SendToMerchantAsync(
        string merchantEmail,
        Guid transactionId,
        string message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[NOTIFICAÇÃO] Para comerciante {Merchant}: {Message} (TransactionId: {TransactionId})",
            merchantEmail, message, transactionId);

        var notification = new Notification(transactionId, merchantEmail, message);
        notification.MarkSent();

        await _transactionRepository.AddNotificationAsync(notification, cancellationToken);
    }
}
