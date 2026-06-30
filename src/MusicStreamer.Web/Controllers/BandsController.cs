using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class BandsController : Controller
{
    private readonly ApiClient _apiClient;
    public BandsController(ApiClient apiClient) => _apiClient = apiClient;

    public async Task<IActionResult> Index()
    {
        var bands = await _apiClient.GetAsync<JsonElement[]>("/api/bands");
        return View(bands ?? Array.Empty<JsonElement>());
    }

    public async Task<IActionResult> Details(string id)
    {
        var band = await _apiClient.GetAsync<JsonElement>($"/api/bands/{id}");
        return View(band);
    }

    public async Task<IActionResult> Search(string term = "", int page = 1)
    {
        var result = await _apiClient.GetAsync<JsonElement>($"/api/bands/search?term={Uri.EscapeDataString(term)}&page={page}&pageSize=10");
        ViewBag.Term = term;
        return View(result);
    }
}
