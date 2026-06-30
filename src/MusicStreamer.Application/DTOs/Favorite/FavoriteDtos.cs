namespace MusicStreamer.Application.DTOs.Favorite;

public record FavoriteMusicResponse(
    Guid MusicId,
    string Title,
    string FormattedDuration,
    string AlbumTitle,
    string BandName,
    DateTime CreatedAt
);

public record FavoriteBandResponse(
    Guid BandId,
    string Name,
    string? Description,
    DateTime CreatedAt
);
