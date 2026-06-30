using MusicStreamer.Application.DTOs.Band;

namespace MusicStreamer.Application.Interfaces.Services;

public interface IBandService
{
    Task<IEnumerable<BandResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BandDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<BandResponse>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BandResponse> CreateAsync(CreateBandRequest request, CancellationToken cancellationToken = default);
    Task<BandResponse> UpdateAsync(Guid id, UpdateBandRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
