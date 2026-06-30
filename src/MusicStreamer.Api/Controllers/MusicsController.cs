using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.DTOs.Music;
using MusicStreamer.Application.Interfaces.Services;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/musics")]
[Produces("application/json")]
public class MusicsController : ControllerBase
{
    private readonly IMusicService _musicService;

    public MusicsController(IMusicService musicService)
    {
        _musicService = musicService;
    }

    /// <summary>Lista todas as músicas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MusicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var musics = await _musicService.GetAllAsync(cancellationToken);
        return Ok(musics);
    }

    /// <summary>Busca uma música por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MusicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var music = await _musicService.GetByIdAsync(id, cancellationToken);
        return Ok(music);
    }

    /// <summary>Busca músicas por termo</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<MusicResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string term = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _musicService.SearchAsync(term, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Cria uma música (requer perfil Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MusicResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMusicRequest request, CancellationToken cancellationToken)
    {
        var music = await _musicService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = music.Id }, music);
    }

    /// <summary>Atualiza uma música (requer perfil Admin)</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MusicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMusicRequest request, CancellationToken cancellationToken)
    {
        var music = await _musicService.UpdateAsync(id, request, cancellationToken);
        return Ok(music);
    }
}
