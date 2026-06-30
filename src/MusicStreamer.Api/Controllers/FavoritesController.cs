using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Favorite;
using MusicStreamer.Application.Interfaces.Services;
using System.Security.Claims;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
[Produces("application/json")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Lista músicas favoritas do usuário</summary>
    [HttpGet("musics")]
    [ProducesResponseType(typeof(IEnumerable<FavoriteMusicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavoriteMusics(CancellationToken cancellationToken)
    {
        var favorites = await _favoriteService.GetFavoriteMusicsAsync(CurrentUserId, cancellationToken);
        return Ok(favorites);
    }

    /// <summary>Adiciona uma música aos favoritos</summary>
    [HttpPost("musics/{musicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavoriteMusic(Guid musicId, CancellationToken cancellationToken)
    {
        await _favoriteService.AddFavoriteMusicAsync(CurrentUserId, musicId, cancellationToken);
        return NoContent();
    }

    /// <summary>Remove uma música dos favoritos</summary>
    [HttpDelete("musics/{musicId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavoriteMusic(Guid musicId, CancellationToken cancellationToken)
    {
        await _favoriteService.RemoveFavoriteMusicAsync(CurrentUserId, musicId, cancellationToken);
        return NoContent();
    }

    /// <summary>Lista bandas favoritas do usuário</summary>
    [HttpGet("bands")]
    [ProducesResponseType(typeof(IEnumerable<FavoriteBandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFavoriteBands(CancellationToken cancellationToken)
    {
        var favorites = await _favoriteService.GetFavoriteBandsAsync(CurrentUserId, cancellationToken);
        return Ok(favorites);
    }

    /// <summary>Adiciona uma banda aos favoritos</summary>
    [HttpPost("bands/{bandId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavoriteBand(Guid bandId, CancellationToken cancellationToken)
    {
        await _favoriteService.AddFavoriteBandAsync(CurrentUserId, bandId, cancellationToken);
        return NoContent();
    }

    /// <summary>Remove uma banda dos favoritos</summary>
    [HttpDelete("bands/{bandId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFavoriteBand(Guid bandId, CancellationToken cancellationToken)
    {
        await _favoriteService.RemoveFavoriteBandAsync(CurrentUserId, bandId, cancellationToken);
        return NoContent();
    }
}
