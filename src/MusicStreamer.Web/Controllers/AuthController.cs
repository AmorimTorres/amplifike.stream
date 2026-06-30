using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Web.Services;
using System.Text.Json;

namespace MusicStreamer.Web.Controllers;

public class AuthController : Controller
{
    private readonly ApiClient _apiClient;

    public AuthController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string name, string email, string password)
    {
        var (success, body, error) = await _apiClient.PostRawAsync(
            "/api/auth/register",
            new { name, email, password });

        if (!success)
        {
            ViewBag.Error = error;
            return View();
        }

        ParseAndStoreToken(body, email);
        TempData["Success"] = "Conta criada com sucesso! Bem-vindo(a)!";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var (success, body, error) = await _apiClient.PostRawAsync(
            "/api/auth/login",
            new { email, password });

        if (!success)
        {
            ViewBag.Error = error ?? "E-mail ou senha inválidos.";
            return View();
        }

        ParseAndStoreToken(body, email);
        TempData["Success"] = "Login realizado com sucesso!";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private void ParseAndStoreToken(string? body, string fallbackEmail)
    {
        if (string.IsNullOrEmpty(body)) return;
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var token = root.TryGetProperty("token", out var t) ? t.GetString() : null;
            var userName = root.TryGetProperty("name", out var n) ? n.GetString() : fallbackEmail;
            var role = root.TryGetProperty("role", out var r) ? r.GetString() : "User";

            HttpContext.Session.SetString("JwtToken", token ?? "");
            HttpContext.Session.SetString("UserName", userName ?? "");
            HttpContext.Session.SetString("UserRole", role ?? "User");
        }
        catch { }
    }
}
