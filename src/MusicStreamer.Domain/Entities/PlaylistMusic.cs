namespace MusicStreamer.Domain.Entities;

public class PlaylistMusic
{
    public Guid PlaylistId { get; private set; }
    public Guid MusicId { get; private set; }
    public DateTime AddedAt { get; private set; }

    // Navigation
    public Playlist? Playlist { get; private set; }
    public Music? Music { get; private set; }

    private PlaylistMusic() { }

    public PlaylistMusic(Guid playlistId, Guid musicId)
    {
        PlaylistId = playlistId;
        MusicId = musicId;
        AddedAt = DateTime.UtcNow;
    }
}
