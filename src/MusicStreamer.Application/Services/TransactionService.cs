using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Transaction;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Enums;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TransactionService> _logger;

    // Business rule constants
    private const decimal MaxTransactionAmount = 10000m;
    private const int MinIntervalBetweenTransactionsMinutes = 1;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        IConfiguration configuration,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TransactionResponse> AuthorizeAsync(
        Guid userId, AuthorizeTransactionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Solicitação de transação para usuário {UserId}: {Merchant} R$ {Amount}",
            userId, request.Merchant, request.Amount);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário", userId);

        var transaction = new Transaction(userId, request.Merchant, request.Amount, request.RequestedAt);
        string? denialReason = null;

        // Validate account status
        if (!user.IsActive())
        {
            denialReason = "Conta do usuário não está ativa.";
        }
        // Validate amount
        else if (request.Amount <= 0)
        {
            denialReason = "Valor da transação deve ser maior que zero.";
        }
        else if (request.Amount > MaxTransactionAmount)
        {
            denialReason = $"Valor excede o limite máximo por transação de R$ {MaxTransactionAmount:N2}.";
        }
        else
        {
            // Check last transaction
            var lastTransaction = await _transactionRepository.GetLastTransactionByUserIdAsync(userId, cancellationToken);

            if (lastTransaction is not null)
            {
                // Check minimum interval
                var timeSinceLast = request.RequestedAt - lastTransaction.RequestedAt;
                if (timeSinceLast.TotalMinutes < MinIntervalBetweenTransactionsMinutes)
                {
                    denialReason = $"Intervalo mínimo entre transações é de {MinIntervalBetweenTransactionsMinutes} minuto(s).";
                }
                // Check for duplicate
                else if (lastTransaction.Merchant == request.Merchant &&
                         lastTransaction.Amount == request.Amount &&
                         (request.RequestedAt - lastTransaction.RequestedAt).TotalMinutes < 5)
                {
                    denialReason = "Transação duplicada detectada (mesmo comerciante e valor nos últimos 5 minutos).";
                }
                // If last transaction was denied, allow retry (no block)
            }
        }

        if (denialReason is not null)
        {
            transaction.Deny(denialReason);
            _logger.LogWarning("Transação negada para {UserId}: {Reason}", userId, denialReason);
        }
        else
        {
            transaction.Authorize();
            _logger.LogInformation("Transação autorizada: {TransactionId}", transaction.Id);
        }

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        // Send notifications only on authorization
        if (transaction.Status == TransactionStatus.Authorized)
        {
            var userMsg = $"Transação autorizada: R$ {request.Amount:N2} para {request.Merchant}.";
            var merchantMsg = $"Pagamento de R$ {request.Amount:N2} autorizado pelo cliente {user.Email}.";

            await _notificationService.SendToUserAsync(userId, user.Email, transaction.Id, userMsg, cancellationToken);
            await _notificationService.SendToMerchantAsync($"contato@{request.Merchant.ToLower().Replace(" ", "")}.com",
                transaction.Id, merchantMsg, cancellationToken);
        }

        return MapToResponse(transaction);
    }

    public async Task<IEnumerable<TransactionResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId, cancellationToken);
        return transactions.Select(MapToResponse);
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Transação", id);

        if (transaction.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para acessar esta transação.");

        return MapToResponse(transaction);
    }

    private static TransactionResponse MapToResponse(Transaction t) =>
        new(t.Id, t.UserId, t.Merchant, t.Amount, t.RequestedAt, t.AuthorizedAt, t.Status.ToString(), t.DenialReason);
}
