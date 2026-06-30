using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.Interfaces.Services;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/search")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly IBandService _bandService;
    private readonly IMusicService _musicService;

    public SearchController(IBandService bandService, IMusicService musicService)
    {
        _bandService = bandService;
        _musicService = musicService;
    }

    /// <summary>Busca global de bandas e músicas</summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string term = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var bands = await _bandService.SearchAsync(term, page, pageSize, cancellationToken);
        var musics = await _musicService.SearchAsync(term, page, pageSize, cancellationToken);

        return Ok(new { bands, musics });
    }
}
