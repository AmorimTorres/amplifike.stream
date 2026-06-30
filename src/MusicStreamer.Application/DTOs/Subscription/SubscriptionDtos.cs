using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Subscription;

public record SubscriptionPlanResponse(
    Guid Id,
    string Name,
    decimal Price,
    int DurationInDays,
    bool IsActive
);

public record UserSubscriptionResponse(
    Guid Id,
    Guid SubscriptionPlanId,
    string PlanName,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    int DaysRemaining
);

public record CreateSubscriptionRequest
{
    [Required(ErrorMessage = "Id do plano é obrigatório.")]
    public Guid PlanId { get; init; }
}
