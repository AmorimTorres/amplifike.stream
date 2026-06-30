using Microsoft.AspNetCore.Mvc;

namespace MusicStreamer.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
