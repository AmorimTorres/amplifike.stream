using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface IBandRepository
{
    Task<Band?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Band>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Band> Items, int TotalCount)> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Band band, CancellationToken cancellationToken = default);
    Task UpdateAsync(Band band, CancellationToken cancellationToken = default);
    Task DeleteAsync(Band band, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
