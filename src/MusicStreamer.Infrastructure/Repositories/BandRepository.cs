using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;

namespace MusicStreamer.Infrastructure.Repositories;

public class BandRepository : IBandRepository
{
    private readonly MusicStreamerDbContext _context;

    public BandRepository(MusicStreamerDbContext context)
    {
        _context = context;
    }

    public async Task<Band?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Bands
            .AsNoTracking()
            .Include(b => b.Albums)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IEnumerable<Band>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Bands
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Band> Items, int TotalCount)> SearchAsync(
        string term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Bands
            .AsNoTracking()
            .Where(b => b.Name.Contains(term));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Band band, CancellationToken cancellationToken = default)
    {
        await _context.Bands.AddAsync(band, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Band band, CancellationToken cancellationToken = default)
    {
        _context.Bands.Update(band);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Band band, CancellationToken cancellationToken = default)
    {
        _context.Bands.Remove(band);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Bands.AnyAsync(b => b.Id == id, cancellationToken);
}
