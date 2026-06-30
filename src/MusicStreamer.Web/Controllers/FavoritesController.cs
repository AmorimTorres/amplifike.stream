using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class FavoritesController : Controller
{
    private readonly ApiClient _apiClient;
    public FavoritesController(ApiClient apiClient) => _apiClient = apiClient;

    private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var musics = await _apiClient.GetAsync<JsonElement[]>("/api/favorites/musics");
        var bands = await _apiClient.GetAsync<JsonElement[]>("/api/favorites/bands");
        ViewBag.FavoriteMusics = musics ?? Array.Empty<JsonElement>();
        ViewBag.FavoriteBands = bands ?? Array.Empty<JsonElement>();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddMusic(string musicId, string returnUrl = "/")
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, _, error) = await _apiClient.PostRawAsync($"/api/favorites/musics/{musicId}", new { });
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Música adicionada aos favoritos!";
        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMusic(string musicId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        await _apiClient.DeleteAsync($"/api/favorites/musics/{musicId}");
        TempData["Success"] = "Música removida dos favoritos.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddBand(string bandId, string returnUrl = "/")
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, _, error) = await _apiClient.PostRawAsync($"/api/favorites/bands/{bandId}", new { });
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Banda adicionada aos favoritos!";
        return LocalRedirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveBand(string bandId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        await _apiClient.DeleteAsync($"/api/favorites/bands/{bandId}");
        TempData["Success"] = "Banda removida dos favoritos.";
        return RedirectToAction("Index");
    }
}
