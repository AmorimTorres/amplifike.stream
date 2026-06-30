using MusicStreamer.Application.DTOs.Music;
using MusicStreamer.Application.DTOs.Band;

namespace MusicStreamer.Application.Interfaces.Services;

public interface IMusicService
{
    Task<IEnumerable<MusicResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MusicResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<MusicResponse>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<MusicResponse> CreateAsync(CreateMusicRequest request, CancellationToken cancellationToken = default);
    Task<MusicResponse> UpdateAsync(Guid id, UpdateMusicRequest request, CancellationToken cancellationToken = default);
}
