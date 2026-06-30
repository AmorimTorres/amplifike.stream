using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class BandService : IBandService
{
    private readonly IBandRepository _bandRepository;
    private readonly ILogger<BandService> _logger;

    public BandService(IBandRepository bandRepository, ILogger<BandService> logger)
    {
        _bandRepository = bandRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<BandResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bands = await _bandRepository.GetAllAsync(cancellationToken);
        return bands.Select(MapToResponse);
    }

    public async Task<BandDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Banda", id);

        return new BandDetailResponse(
            band.Id,
            band.Name,
            band.Description,
            band.CreatedAt,
            band.Albums.Select(a => new AlbumSummaryResponse(a.Id, a.Title, a.ReleaseYear))
        );
    }

    public async Task<PagedResponse<BandResponse>> SearchAsync(
        string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var (items, totalCount) = await _bandRepository.SearchAsync(term, page, pageSize, cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResponse<BandResponse>(
            items.Select(MapToResponse),
            page,
            pageSize,
            totalCount,
            totalPages
        );
    }

    public async Task<BandResponse> CreateAsync(CreateBandRequest request, CancellationToken cancellationToken = default)
    {
        var band = new Band(request.Name, request.Description);
        await _bandRepository.AddAsync(band, cancellationToken);
        _logger.LogInformation("Banda criada: {BandId} - {Name}", band.Id, band.Name);
        return MapToResponse(band);
    }

    public async Task<BandResponse> UpdateAsync(Guid id, UpdateBandRequest request, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Banda", id);

        band.Update(request.Name, request.Description);
        await _bandRepository.UpdateAsync(band, cancellationToken);

        return MapToResponse(band);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var band = await _bandRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Banda", id);

        await _bandRepository.DeleteAsync(band, cancellationToken);
        _logger.LogInformation("Banda excluída: {BandId}", id);
    }

    private static BandResponse MapToResponse(Band band) =>
        new(band.Id, band.Name, band.Description, band.CreatedAt, band.Albums.Count);
}
