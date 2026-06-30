using MusicStreamer.Application.DTOs.Transaction;

namespace MusicStreamer.Application.Interfaces.Services;

public interface ITransactionService
{
    Task<TransactionResponse> AuthorizeAsync(Guid userId, AuthorizeTransactionRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransactionResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TransactionResponse> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
