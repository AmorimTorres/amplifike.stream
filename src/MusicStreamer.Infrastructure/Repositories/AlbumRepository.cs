using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly MusicStreamerDbContext _context;

    public AlbumRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Albums
            .AsNoTracking()
            .Include(a => a.Band)
            .Include(a => a.Musics)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Albums
            .AsNoTracking()
            .Include(a => a.Band)
            .OrderBy(a => a.Title)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Album>> GetByBandIdAsync(Guid bandId, CancellationToken cancellationToken = default)
        => await _context.Albums
            .AsNoTracking()
            .Where(a => a.BandId == bandId)
            .OrderBy(a => a.ReleaseYear)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Album album, CancellationToken cancellationToken = default)
    {
        await _context.Albums.AddAsync(album, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Album album, CancellationToken cancellationToken = default)
    {
        _context.Albums.Update(album);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Albums.AnyAsync(a => a.Id == id, cancellationToken);
}
