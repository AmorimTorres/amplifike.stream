using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.Interfaces.Services;

namespace MusicStreamer.Api.Controllers;

[ApiController]
[Route("api/bands")]
[Produces("application/json")]
public class BandsController : ControllerBase
{
    private readonly IBandService _bandService;

    public BandsController(IBandService bandService)
    {
        _bandService = bandService;
    }

    /// <summary>Lista todas as bandas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var bands = await _bandService.GetAllAsync(cancellationToken);
        return Ok(bands);
    }

    /// <summary>Busca uma banda por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BandDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var band = await _bandService.GetByIdAsync(id, cancellationToken);
        return Ok(band);
    }

    /// <summary>Busca bandas por termo</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResponse<BandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string term = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _bandService.SearchAsync(term, page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Cria uma banda (requer perfil Admin)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateBandRequest request, CancellationToken cancellationToken)
    {
        var band = await _bandService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = band.Id }, band);
    }

    /// <summary>Atualiza uma banda (requer perfil Admin)</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBandRequest request, CancellationToken cancellationToken)
    {
        var band = await _bandService.UpdateAsync(id, request, cancellationToken);
        return Ok(band);
    }

    /// <summary>Remove uma banda (requer perfil Admin)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _bandService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
