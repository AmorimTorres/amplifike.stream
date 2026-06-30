using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Subscription;
using MusicStreamer.Application.Interfaces.Services;
using System.Security.Claims;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lista todos os planos de assinatura ativos</summary>
    [HttpGet("subscription-plans")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
    {
        var plans = await _subscriptionService.GetPlansAsync(cancellationToken);
        return Ok(plans);
    }

    /// <summary>Assina um plano</summary>
    [HttpPost("subscriptions")]
    [Authorize]
    [ProducesResponseType(typeof(UserSubscriptionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Subscribe(
        [FromBody] CreateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionService.SubscribeAsync(CurrentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetCurrent), subscription);
    }

    /// <summary>Consulta a assinatura atual do usuário</summary>
    [HttpGet("subscriptions/current")]
    [Authorize]
    [ProducesResponseType(typeof(UserSubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionService.GetCurrentSubscriptionAsync(CurrentUserId, cancellationToken);
        if (subscription is null) return NoContent();
        return Ok(subscription);
    }
}
