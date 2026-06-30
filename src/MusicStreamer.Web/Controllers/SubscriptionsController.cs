using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class SubscriptionsController : Controller
{
    private readonly ApiClient _apiClient;
    public SubscriptionsController(ApiClient apiClient) => _apiClient = apiClient;

    private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        var plans = await _apiClient.GetAsync<JsonElement[]>("/api/subscription-plans");
        if (IsLoggedIn)
        {
            var current = await _apiClient.GetAsync<JsonElement>("/api/subscriptions/current");
            ViewBag.CurrentSubscription = (current.ValueKind == JsonValueKind.Undefined) ? (JsonElement?)null : current;
        }
        return View(plans ?? Array.Empty<JsonElement>());
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string planId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, _, error) = await _apiClient.PostRawAsync("/api/subscriptions", new { planId = Guid.Parse(planId) });
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Assinatura realizada com sucesso!";
        return RedirectToAction("Index");
    }
}
