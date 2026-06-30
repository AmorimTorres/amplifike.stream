using MusicStreamer.Application.DTOs.Album;

namespace MusicStreamer.Application.Interfaces.Services;

public interface IAlbumService
{
    Task<IEnumerable<AlbumResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AlbumDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlbumResponse> CreateAsync(CreateAlbumRequest request, CancellationToken cancellationToken = default);
}
