using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Album;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;
    private readonly IBandRepository _bandRepository;
    private readonly ILogger<AlbumService> _logger;

    public AlbumService(
        IAlbumRepository albumRepository,
        IBandRepository bandRepository,
        ILogger<AlbumService> logger)
    {
        _albumRepository = albumRepository;
        _bandRepository = bandRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AlbumResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var albums = await _albumRepository.GetAllAsync(cancellationToken);
        return albums.Select(MapToResponse);
    }

    public async Task<AlbumDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var album = await _albumRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Álbum", id);

        return new AlbumDetailResponse(
            album.Id,
            album.Title,
            album.ReleaseYear,
            album.BandId,
            album.Band?.Name ?? string.Empty,
            album.Musics.Select(m => new MusicSummaryResponse(m.Id, m.Title, m.DurationInSeconds))
        );
    }

    public async Task<AlbumResponse> CreateAsync(CreateAlbumRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _bandRepository.ExistsAsync(request.BandId, cancellationToken))
            throw new NotFoundException("Banda", request.BandId);

        var album = new Album(request.Title, request.ReleaseYear, request.BandId);
        await _albumRepository.AddAsync(album, cancellationToken);

        _logger.LogInformation("Álbum criado: {AlbumId} - {Title}", album.Id, album.Title);

        // Reload with band info
        var created = await _albumRepository.GetByIdAsync(album.Id, cancellationToken);
        return MapToResponse(created!);
    }

    private static AlbumResponse MapToResponse(Album album) =>
        new(album.Id, album.Title, album.ReleaseYear, album.BandId, album.Band?.Name ?? string.Empty);
}
