using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class PlaylistsController : Controller
{
    private readonly ApiClient _apiClient;
    public PlaylistsController(ApiClient apiClient) => _apiClient = apiClient;

    private bool IsLoggedIn => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var playlists = await _apiClient.GetAsync<JsonElement[]>("/api/playlists");
        return View(playlists ?? Array.Empty<JsonElement>());
    }

    public async Task<IActionResult> Details(string id)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var playlist = await _apiClient.GetAsync<JsonElement>($"/api/playlists/{id}");
        return View(playlist);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, _, error) = await _apiClient.PostRawAsync("/api/playlists", new { name });
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Playlist criada com sucesso!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddMusic(string playlistId, string musicId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, _, error) = await _apiClient.PostRawAsync(
            $"/api/playlists/{playlistId}/musics/{musicId}", new { });
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Música adicionada à playlist!";
        return RedirectToAction("Details", new { id = playlistId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMusic(string playlistId, string musicId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, error) = await _apiClient.DeleteAsync($"/api/playlists/{playlistId}/musics/{musicId}");
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Música removida da playlist.";
        return RedirectToAction("Details", new { id = playlistId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Auth");
        var (success, error) = await _apiClient.DeleteAsync($"/api/playlists/{id}");
        if (!success) TempData["Error"] = error;
        else TempData["Success"] = "Playlist excluída.";
        return RedirectToAction("Index");
    }
}
