using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface IFavoriteRepository
{
    // Music favorites
    Task<FavoriteMusic?> GetFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FavoriteMusic>> GetFavoriteMusicsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddFavoriteMusicAsync(FavoriteMusic favoriteMusic, CancellationToken cancellationToken = default);
    Task RemoveFavoriteMusicAsync(FavoriteMusic favoriteMusic, CancellationToken cancellationToken = default);

    // Band favorites
    Task<FavoriteBand?> GetFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FavoriteBand>> GetFavoriteBandsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddFavoriteBandAsync(FavoriteBand favoriteBand, CancellationToken cancellationToken = default);
    Task RemoveFavoriteBandAsync(FavoriteBand favoriteBand, CancellationToken cancellationToken = default);
}
