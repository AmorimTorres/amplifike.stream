using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface IMusicRepository
{
    Task<Music?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Music>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<Music> Items, int TotalCount)> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Music>> GetByAlbumIdAsync(Guid albumId, CancellationToken cancellationToken = default);
    Task AddAsync(Music music, CancellationToken cancellationToken = default);
    Task UpdateAsync(Music music, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
