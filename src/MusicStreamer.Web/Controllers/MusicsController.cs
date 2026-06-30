using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class MusicsController : Controller
{
    private readonly ApiClient _apiClient;
    public MusicsController(ApiClient apiClient) => _apiClient = apiClient;

    public async Task<IActionResult> Index()
    {
        var musics = await _apiClient.GetAsync<JsonElement[]>("/api/musics");
        return View(musics ?? Array.Empty<JsonElement>());
    }

    public async Task<IActionResult> Search(string term = "", int page = 1)
    {
        var result = await _apiClient.GetAsync<JsonElement>($"/api/musics/search?term={Uri.EscapeDataString(term)}&page={page}&pageSize=10");
        ViewBag.Term = term;
        return View(result);
    }
}
