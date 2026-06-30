using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Subscription;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        ILogger<SubscriptionService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SubscriptionPlanResponse>> GetPlansAsync(CancellationToken cancellationToken = default)
    {
        var plans = await _subscriptionRepository.GetActivePlansAsync(cancellationToken);
        return plans.Select(p => new SubscriptionPlanResponse(p.Id, p.Name, p.Price, p.DurationInDays, p.IsActive));
    }

    public async Task<UserSubscriptionResponse> SubscribeAsync(
        Guid userId, CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário", userId);

        if (!user.IsActive())
            throw new BusinessRuleException("Conta inativa não pode assinar planos.");

        var plan = await _subscriptionRepository.GetPlanByIdAsync(request.PlanId, cancellationToken)
            ?? throw new NotFoundException("Plano de assinatura", request.PlanId);

        if (!plan.IsActive)
            throw new BusinessRuleException("O plano selecionado não está ativo.");

        var existing = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Usuário já possui uma assinatura ativa. Aguarde o vencimento para renovar.");

        var subscription = new UserSubscription(userId, plan.Id, plan.DurationInDays);
        await _subscriptionRepository.AddSubscriptionAsync(subscription, cancellationToken);

        _logger.LogInformation("Assinatura criada: Usuário {UserId}, Plano {PlanName}", userId, plan.Name);

        return MapToResponse(subscription, plan);
    }

    public async Task<UserSubscriptionResponse?> GetCurrentSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, cancellationToken);
        if (subscription is null) return null;

        return MapToResponse(subscription, subscription.SubscriptionPlan!);
    }

    private static UserSubscriptionResponse MapToResponse(UserSubscription sub, SubscriptionPlan plan)
    {
        var daysRemaining = Math.Max(0, (int)(sub.EndDate - DateTime.UtcNow).TotalDays);
        return new UserSubscriptionResponse(
            sub.Id, sub.SubscriptionPlanId, plan.Name, plan.Price,
            sub.StartDate, sub.EndDate, sub.IsActive, daysRemaining
        );
    }
}
