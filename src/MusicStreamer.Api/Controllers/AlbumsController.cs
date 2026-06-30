using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Album;
using MusicStreamer.Application.Interfaces.Services;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/albums")]
[Produces("application/json")]
public class AlbumsController : ControllerBase
{
    private readonly IAlbumService _albumService;

    public AlbumsController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    /// <summary>Lista todos os álbuns</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlbumResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var albums = await _albumService.GetAllAsync(cancellationToken);
        return Ok(albums);
    }

    /// <summary>Busca um álbum por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlbumDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var album = await _albumService.GetByIdAsync(id, cancellationToken);
        return Ok(album);
    }

    /// <summary>Cria um álbum (requer perfil Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AlbumResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAlbumRequest request, CancellationToken cancellationToken)
    {
        var album = await _albumService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = album.Id }, album);
    }
}
