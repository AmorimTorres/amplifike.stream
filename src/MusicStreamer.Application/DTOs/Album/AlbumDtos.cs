using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Album;

public record AlbumResponse(
    Guid Id,
    string Title,
    int ReleaseYear,
    Guid BandId,
    string BandName
);

public record AlbumDetailResponse(
    Guid Id,
    string Title,
    int ReleaseYear,
    Guid BandId,
    string BandName,
    IEnumerable<MusicSummaryResponse> Musics
);

public record MusicSummaryResponse(
    Guid Id,
    string Title,
    int DurationInSeconds
);

public record CreateAlbumRequest
{
    [Required(ErrorMessage = "Título do álbum é obrigatório.")]
    [MaxLength(300)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [Range(1900, 2100, ErrorMessage = "Ano de lançamento inválido.")]
    public int ReleaseYear { get; init; }

    [Required(ErrorMessage = "Id da banda é obrigatório.")]
    public Guid BandId { get; init; }
}
