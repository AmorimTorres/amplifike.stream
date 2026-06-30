using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Playlist;

public record PlaylistResponse(
    Guid Id,
    string Name,
    Guid UserId,
    DateTime CreatedAt,
    int MusicCount
);

public record PlaylistDetailResponse(
    Guid Id,
    string Name,
    Guid UserId,
    DateTime CreatedAt,
    IEnumerable<PlaylistMusicResponse> Musics
);

public record PlaylistMusicResponse(
    Guid MusicId,
    string Title,
    int DurationInSeconds,
    string FormattedDuration,
    string AlbumTitle,
    string BandName,
    DateTime AddedAt
);

public record CreatePlaylistRequest
{
    [Required(ErrorMessage = "Nome da playlist é obrigatório.")]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;
}
