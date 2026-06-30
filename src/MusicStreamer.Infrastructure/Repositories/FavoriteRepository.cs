using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly MusicStreamerDbContext _context;

    public FavoriteRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<FavoriteMusic?> GetFavoriteMusicAsync(Guid userId, Guid musicId, CancellationToken cancellationToken = default)
        => await _context.FavoriteMusics
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MusicId == musicId, cancellationToken);

    public async Task<IEnumerable<FavoriteMusic>> GetFavoriteMusicsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.FavoriteMusics
            .AsNoTracking()
            .Include(f => f.Music)
                .ThenInclude(m => m!.Album)
                    .ThenInclude(a => a!.Band)
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddFavoriteMusicAsync(FavoriteMusic favoriteMusic, CancellationToken cancellationToken = default)
    {
        await _context.FavoriteMusics.AddAsync(favoriteMusic, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFavoriteMusicAsync(FavoriteMusic favoriteMusic, CancellationToken cancellationToken = default)
    {
        _context.FavoriteMusics.Remove(favoriteMusic);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FavoriteBand?> GetFavoriteBandAsync(Guid userId, Guid bandId, CancellationToken cancellationToken = default)
        => await _context.FavoriteBands
            .FirstOrDefaultAsync(f => f.UserId == userId && f.BandId == bandId, cancellationToken);

    public async Task<IEnumerable<FavoriteBand>> GetFavoriteBandsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.FavoriteBands
            .AsNoTracking()
            .Include(f => f.Band)
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddFavoriteBandAsync(FavoriteBand favoriteBand, CancellationToken cancellationToken = default)
    {
        await _context.FavoriteBands.AddAsync(favoriteBand, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFavoriteBandAsync(FavoriteBand favoriteBand, CancellationToken cancellationToken = default)
    {
        _context.FavoriteBands.Remove(favoriteBand);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
