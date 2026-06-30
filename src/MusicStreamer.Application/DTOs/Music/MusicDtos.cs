using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Music;

public record MusicResponse(
    Guid Id,
    string Title,
    int DurationInSeconds,
    string FormattedDuration,
    Guid AlbumId,
    string AlbumTitle,
    Guid BandId,
    string BandName,
    DateTime CreatedAt
);

public record CreateMusicRequest
{
    [Required(ErrorMessage = "Título da música é obrigatório.")]
    [MaxLength(300)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [Range(1, 7200, ErrorMessage = "Duração deve ser entre 1 e 7200 segundos.")]
    public int DurationInSeconds { get; init; }

    [Required(ErrorMessage = "Id do álbum é obrigatório.")]
    public Guid AlbumId { get; init; }
}

public record UpdateMusicRequest
{
    [Required(ErrorMessage = "Título da música é obrigatório.")]
    [MaxLength(300)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [Range(1, 7200)]
    public int DurationInSeconds { get; init; }
}
