using MusicStreamer.Application.DTOs.Favorite;

namespace MusicStreamer.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteMusicResponse>> GetFavoriteMusicsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default);

    Task<IEnumerable<FavoriteBandResponse>> GetFavoriteBandsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default);
}
