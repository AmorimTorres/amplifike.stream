using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.DTOs.Music;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class MusicService : IMusicService
{
    private readonly IMusicRepository _musicRepository;
    private readonly IAlbumRepository _albumRepository;
    private readonly ILogger<MusicService> _logger;

    public MusicService(
        IMusicRepository musicRepository,
        IAlbumRepository albumRepository,
        ILogger<MusicService> logger)
    {
        _musicRepository = musicRepository;
        _albumRepository = albumRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MusicResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var musics = await _musicRepository.GetAllAsync(cancellationToken);
        return musics.Select(MapToResponse);
    }

    public async Task<MusicResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var music = await _musicRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Música", id);

        return MapToResponse(music);
    }

    public async Task<PagedResponse<MusicResponse>> SearchAsync(
        string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var (items, totalCount) = await _musicRepository.SearchAsync(term, page, pageSize, cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResponse<MusicResponse>(
            items.Select(MapToResponse),
            page,
            pageSize,
            totalCount,
            totalPages
        );
    }

    public async Task<MusicResponse> CreateAsync(CreateMusicRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _albumRepository.ExistsAsync(request.AlbumId, cancellationToken))
            throw new NotFoundException("Álbum", request.AlbumId);

        var music = new Music(request.Title, request.DurationInSeconds, request.AlbumId);
        await _musicRepository.AddAsync(music, cancellationToken);

        _logger.LogInformation("Música criada: {MusicId} - {Title}", music.Id, music.Title);

        var created = await _musicRepository.GetByIdAsync(music.Id, cancellationToken);
        return MapToResponse(created!);
    }

    public async Task<MusicResponse> UpdateAsync(Guid id, UpdateMusicRequest request, CancellationToken cancellationToken = default)
    {
        var music = await _musicRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Música", id);

        music.Update(request.Title, request.DurationInSeconds);
        await _musicRepository.UpdateAsync(music, cancellationToken);

        var updated = await _musicRepository.GetByIdAsync(id, cancellationToken);
        return MapToResponse(updated!);
    }

    private static MusicResponse MapToResponse(Music music)
    {
        var minutes = music.DurationInSeconds / 60;
        var seconds = music.DurationInSeconds % 60;
        var formatted = $"{minutes}:{seconds:D2}";

        return new MusicResponse(
            music.Id,
            music.Title,
            music.DurationInSeconds,
            formatted,
            music.AlbumId,
            music.Album?.Title ?? string.Empty,
            music.Album?.BandId ?? Guid.Empty,
            music.Album?.Band?.Name ?? string.Empty,
            music.CreatedAt
        );
    }
}
