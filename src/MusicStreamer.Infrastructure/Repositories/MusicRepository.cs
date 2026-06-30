using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class MusicRepository : IMusicRepository
{
    private readonly MusicStreamerDbContext _context;

    public MusicRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<Music?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Musics
            .AsNoTracking()
            .Include(m => m.Album)
                .ThenInclude(a => a!.Band)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IEnumerable<Music>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Musics
            .AsNoTracking()
            .Include(m => m.Album)
                .ThenInclude(a => a!.Band)
            .OrderBy(m => m.Title)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Music> Items, int TotalCount)> SearchAsync(
        string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Musics
            .AsNoTracking()
            .Include(m => m.Album)
                .ThenInclude(a => a!.Band)
            .Where(m => m.Title.Contains(term));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<Music>> GetByAlbumIdAsync(Guid albumId, CancellationToken cancellationToken = default)
        => await _context.Musics
            .AsNoTracking()
            .Where(m => m.AlbumId == albumId)
            .OrderBy(m => m.Title)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Music music, CancellationToken cancellationToken = default)
    {
        await _context.Musics.AddAsync(music, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Music music, CancellationToken cancellationToken = default)
    {
        _context.Musics.Update(music);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Musics.AnyAsync(m => m.Id == id, cancellationToken);
}
