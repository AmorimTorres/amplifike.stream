using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly MusicStreamerDbContext _context;

    public PlaylistRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Playlists
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Playlist?> GetByIdWithMusicsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Playlists
            .AsNoTracking()
            .Include(p => p.PlaylistMusics)
                .ThenInclude(pm => pm.Music)
                    .ThenInclude(m => m!.Album)
                        .ThenInclude(a => a!.Band)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<Playlist>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Playlists
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Playlist playlist, CancellationToken cancellationToken = default)
    {
        await _context.Playlists.AddAsync(playlist, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Playlist playlist, CancellationToken cancellationToken = default)
    {
        _context.Playlists.Update(playlist);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken = default)
    {
        _context.Playlists.Remove(playlist);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMusicAsync(PlaylistMusic playlistMusic, CancellationToken cancellationToken = default)
    {
        await _context.PlaylistMusics.AddAsync(playlistMusic, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveMusicAsync(PlaylistMusic playlistMusic, CancellationToken cancellationToken = default)
    {
        _context.PlaylistMusics.Remove(playlistMusic);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PlaylistMusic?> GetPlaylistMusicAsync(Guid playlistId, Guid musicId, CancellationToken cancellationToken = default)
        => await _context.PlaylistMusics
            .FirstOrDefaultAsync(pm => pm.PlaylistId == playlistId && pm.MusicId == musicId, cancellationToken);
}
