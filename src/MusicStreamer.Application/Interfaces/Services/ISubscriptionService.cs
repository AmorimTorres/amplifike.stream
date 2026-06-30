using MusicStreamer.Application.DTOs.Subscription;

namespace MusicStreamer.Application.Interfaces.Services;

public interface ISubscriptionService
{
    Task<IEnumerable<SubscriptionPlanResponse>> GetPlansAsync(CancellationToken cancellationToken = default);
    Task<UserSubscriptionResponse> SubscribeAsync(Guid userId, CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
    Task<UserSubscriptionResponse?> GetCurrentSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default);
}
