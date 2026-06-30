using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Transaction;

public record AuthorizeTransactionRequest
{
    [Required(ErrorMessage = "Comerciante é obrigatório.")]
    [MaxLength(200)]
    public string Merchant { get; init; } = string.Empty;

    [Required]
    [Range(0.01, 50000, ErrorMessage = "Valor deve ser maior que zero e no máximo R$ 50.000.")]
    public decimal Amount { get; init; }

    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
}

public record TransactionResponse(
    Guid Id,
    Guid UserId,
    string Merchant,
    decimal Amount,
    DateTime RequestedAt,
    DateTime? AuthorizedAt,
    string Status,
    string? DenialReason
);
