using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly MusicStreamerDbContext _context;

    public SubscriptionRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync(CancellationToken cancellationToken = default)
        => await _context.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Price)
            .ToListAsync(cancellationToken);

    public async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.UserSubscriptions
            .AsNoTracking()
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddSubscriptionAsync(UserSubscription subscription, CancellationToken cancellationToken = default)
    {
        await _context.UserSubscriptions.AddAsync(subscription, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSubscriptionAsync(UserSubscription subscription, CancellationToken cancellationToken = default)
    {
        _context.UserSubscriptions.Update(subscription);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
