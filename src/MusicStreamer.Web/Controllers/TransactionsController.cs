using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class TransactionsController : Controller
{
    private readonly ApiClient _apiClient;
    public TransactionsController(ApiClient apiClient) => _apiClient = apiClient;

    private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var transactions = await _apiClient.GetAsync<JsonElement[]>("/api/transactions");
        return View(transactions ?? Array.Empty<JsonElement>());
    }

    [HttpGet]
    public IActionResult Authorize()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Authorize(string merchant, decimal amount)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, body, error) = await _apiClient.PostRawAsync(
            "/api/transactions/authorize",
            new { merchant, amount, requestedAt = DateTime.UtcNow });

        if (!success)
        {
            TempData["Error"] = error;
            return View();
        }

        string status = "?";
        string? denialReason = null;
        if (!string.IsNullOrEmpty(body))
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            status = root.TryGetProperty("status", out var s) ? (s.GetString() ?? "?") : "?";
            denialReason = root.TryGetProperty("denialReason", out var d) && d.ValueKind != JsonValueKind.Null
                ? d.GetString() : null;
        }

        if (status == "Authorized")
            TempData["Success"] = $"Transação autorizada com sucesso! Valor: R$ {amount:N2} para {merchant}.";
        else
            TempData["Error"] = $"Transação negada: {denialReason}";

        return RedirectToAction("Index");
    }
}
