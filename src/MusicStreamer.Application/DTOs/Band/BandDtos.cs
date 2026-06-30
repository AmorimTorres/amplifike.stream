using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Band;

public record BandResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int AlbumCount
);

public record BandDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    IEnumerable<AlbumSummaryResponse> Albums
);

public record AlbumSummaryResponse(
    Guid Id,
    string Title,
    int ReleaseYear
);

public record CreateBandRequest
{
    [Required(ErrorMessage = "Nome da banda é obrigatório.")]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }
}

public record UpdateBandRequest
{
    [Required(ErrorMessage = "Nome da banda é obrigatório.")]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }
}

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
