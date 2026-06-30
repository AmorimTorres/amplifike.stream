using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Playlist;
using MusicStreamer.Application.Interfaces.Services;
using System.Security.Claims;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/playlists")]
[Authorize]
[Produces("application/json")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlistService;

    public PlaylistsController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lista as playlists do usuário autenticado</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlaylistResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var playlists = await _playlistService.GetByUserIdAsync(CurrentUserId, cancellationToken);
        return Ok(playlists);
    }

    /// <summary>Busca uma playlist por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlaylistDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var playlist = await _playlistService.GetByIdAsync(id, CurrentUserId, cancellationToken);
        return Ok(playlist);
    }

    /// <summary>Cria uma playlist</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PlaylistResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePlaylistRequest request, CancellationToken cancellationToken)
    {
        var playlist = await _playlistService.CreateAsync(CurrentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = playlist.Id }, playlist);
    }

    /// <summary>Remove uma playlist</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _playlistService.DeleteAsync(id, CurrentUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>Adiciona uma música à playlist</summary>
    [HttpPost("{playlistId:guid}/musics/{musicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddMusic(Guid playlistId, Guid musicId, CancellationToken cancellationToken)
    {
        await _playlistService.AddMusicAsync(playlistId, musicId, CurrentUserId, cancellationToken);
        return NoContent();
    }

    /// <summary>Remove uma música da playlist</summary>
    [HttpDelete("{playlistId:guid}/musics/{musicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMusic(Guid playlistId, Guid musicId, CancellationToken cancellationToken)
    {
        await _playlistService.RemoveMusicAsync(playlistId, musicId, CurrentUserId, cancellationToken);
        return NoContent();
    }
}
