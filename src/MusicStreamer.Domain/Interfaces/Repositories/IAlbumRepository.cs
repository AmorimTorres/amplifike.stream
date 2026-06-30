using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> GetByBandIdAsync(Guid bandId, CancellationToken cancellationToken = default);
    Task AddAsync(Album album, CancellationToken cancellationToken = default);
    Task UpdateAsync(Album album, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
