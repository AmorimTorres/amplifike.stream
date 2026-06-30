using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync(CancellationToken cancellationToken = default);
    Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddSubscriptionAsync(UserSubscription subscription, CancellationToken cancellationToken = default);
    Task UpdateSubscriptionAsync(UserSubscription subscription, CancellationToken cancellationToken = default);
}
