using MusicStreamer.Application.DTOs.Playlist;

namespace MusicStreamer.Application.Interfaces.Services;

public interface IPlaylistService
{
    Task<IEnumerable<PlaylistResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PlaylistDetailResponse> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<PlaylistResponse> CreateAsync(Guid userId, CreatePlaylistRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task AddMusicAsync(Guid playlistId, Guid musicId, Guid userId, CancellationToken cancellationToken = default);
    Task RemoveMusicAsync(Guid playlistId, Guid musicId, Guid userId, CancellationToken cancellationToken = default);
}
