using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Playlist;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IMusicRepository _musicRepository;
    private readonly ILogger<PlaylistService> _logger;

    public PlaylistService(
        IPlaylistRepository playlistRepository,
        IMusicRepository musicRepository,
        ILogger<PlaylistService> logger)
    {
        _playlistRepository = playlistRepository;
        _musicRepository = musicRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PlaylistResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var playlists = await _playlistRepository.GetByUserIdAsync(userId, cancellationToken);
        return playlists.Select(p => new PlaylistResponse(
            p.Id, p.Name, p.UserId, p.CreatedAt, p.PlaylistMusics.Count));
    }

    public async Task<PlaylistDetailResponse> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _playlistRepository.GetByIdWithMusicsAsync(id, cancellationToken)
            ?? throw new NotFoundException("Playlist", id);

        if (playlist.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para acessar esta playlist.");

        var musics = playlist.PlaylistMusics.Select(pm =>
        {
            var m = pm.Music!;
            var minutes = m.DurationInSeconds / 60;
            var seconds = m.DurationInSeconds % 60;
            return new PlaylistMusicResponse(
                m.Id,
                m.Title,
                m.DurationInSeconds,
                $"{minutes}:{seconds:D2}",
                m.Album?.Title ?? string.Empty,
                m.Album?.Band?.Name ?? string.Empty,
                pm.AddedAt
            );
        });

        return new PlaylistDetailResponse(playlist.Id, playlist.Name, playlist.UserId, playlist.CreatedAt, musics);
    }

    public async Task<PlaylistResponse> CreateAsync(Guid userId, CreatePlaylistRequest request, CancellationToken cancellationToken = default)
    {
        var playlist = new Playlist(request.Name, userId);
        await _playlistRepository.AddAsync(playlist, cancellationToken);

        _logger.LogInformation("Playlist criada: {PlaylistId} para usuário {UserId}", playlist.Id, userId);

        return new PlaylistResponse(playlist.Id, playlist.Name, playlist.UserId, playlist.CreatedAt, 0);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _playlistRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Playlist", id);

        if (playlist.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para excluir esta playlist.");

        await _playlistRepository.DeleteAsync(playlist, cancellationToken);
    }

    public async Task AddMusicAsync(Guid playlistId, Guid musicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId, cancellationToken)
            ?? throw new NotFoundException("Playlist", playlistId);

        if (playlist.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para alterar esta playlist.");

        if (!await _musicRepository.ExistsAsync(musicId, cancellationToken))
            throw new NotFoundException("Música", musicId);

        var existing = await _playlistRepository.GetPlaylistMusicAsync(playlistId, musicId, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Esta música já está na playlist.");

        var playlistMusic = new PlaylistMusic(playlistId, musicId);
        await _playlistRepository.AddMusicAsync(playlistMusic, cancellationToken);
    }

    public async Task RemoveMusicAsync(Guid playlistId, Guid musicId, Guid userId, CancellationToken cancellationToken = default)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId, cancellationToken)
            ?? throw new NotFoundException("Playlist", playlistId);

        if (playlist.UserId != userId)
            throw new ForbiddenException("Você não tem permissão para alterar esta playlist.");

        var playlistMusic = await _playlistRepository.GetPlaylistMusicAsync(playlistId, musicId, cancellationToken)
            ?? throw new NotFoundException("A música não está nesta playlist.");

        await _playlistRepository.RemoveMusicAsync(playlistMusic, cancellationToken);
    }
}
