using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Domain.Interfaces.Repositories;

public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Playlist?> GetByIdWithMusicsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Playlist>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Playlist playlist, CancellationToken cancellationToken = default);
    Task UpdateAsync(Playlist playlist, CancellationToken cancellationToken = default);
    Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken = default);
    Task AddMusicAsync(PlaylistMusic playlistMusic, CancellationToken cancellationToken = default);
    Task RemoveMusicAsync(PlaylistMusic playlistMusic, CancellationToken cancellationToken = default);
    Task<PlaylistMusic?> GetPlaylistMusicAsync(Guid playlistId, Guid musicId, CancellationToken cancellationToken = default);
}
