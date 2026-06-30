using Microsoft.Extensions.Logging;
using MusicStreamer.Application.DTOs.Favorite;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IMusicRepository _musicRepository;
    private readonly IBandRepository _bandRepository;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IMusicRepository musicRepository,
        IBandRepository bandRepository,
        ILogger<FavoriteService> logger)
    {
        _favoriteRepository = favoriteRepository;
        _musicRepository = musicRepository;
        _bandRepository = bandRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FavoriteMusicResponse>> GetFavoriteMusicsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository.GetFavoriteMusicsAsync(userId, cancellationToken);
        return favorites.Select(f =>
        {
            var m = f.Music!;
            var minutes = m.DurationInSeconds / 60;
            var seconds = m.DurationInSeconds % 60;
            return new FavoriteMusicResponse(
                m.Id, m.Title, $"{minutes}:{seconds:D2}",
                m.Album?.Title ?? string.Empty,
                m.Album?.Band?.Name ?? string.Empty,
                f.CreatedAt
            );
        });
    }

    public async Task AddFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default)
    {
        if (!await _musicRepository.ExistsAsync(musicId, cancellationToken))
            throw new NotFoundException("Música", musicId);

        var existing = await _favoriteRepository.GetFavoriteMusicAsync(userId, musicId, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Música já está nos favoritos.");

        await _favoriteRepository.AddFavoriteMusicAsync(new FavoriteMusic(userId, musicId), cancellationToken);
    }

    public async Task RemoveFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRepository.GetFavoriteMusicAsync(userId, musicId, cancellationToken)
            ?? throw new NotFoundException("Música não está nos favoritos.");

        await _favoriteRepository.RemoveFavoriteMusicAsync(favorite, cancellationToken);
    }

    public async Task<IEnumerable<FavoriteBandResponse>> GetFavoriteBandsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository.GetFavoriteBandsAsync(userId, cancellationToken);
        return favorites.Select(f => new FavoriteBandResponse(
            f.Band!.Id, f.Band.Name, f.Band.Description, f.CreatedAt
        ));
    }

    public async Task AddFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default)
    {
        if (!await _bandRepository.ExistsAsync(bandId, cancellationToken))
            throw new NotFoundException("Banda", bandId);

        var existing = await _favoriteRepository.GetFavoriteBandAsync(userId, bandId, cancellationToken);
        if (existing is not null)
            throw new ConflictException("Banda já está nos favoritos.");

        await _favoriteRepository.AddFavoriteBandAsync(new FavoriteBand(userId, bandId), cancellationToken);
    }

    public async Task RemoveFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRepository.GetFavoriteBandAsync(userId, bandId, cancellationToken)
            ?? throw new NotFoundException("Banda não está nos favoritos.");

        await _favoriteRepository.RemoveFavoriteBandAsync(favorite, cancellationToken);
    }
}
