namespace MusicStreamer.Domain.Entities;

public class FavoriteMusic
{
    public Guid UserId { get; private set; }
    public Guid MusicId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User? User { get; private set; }
    public Music? Music { get; private set; }

    private FavoriteMusic() { }

    public FavoriteMusic(Guid userId, Guid musicId)
    {
        UserId = userId;
        MusicId = musicId;
        CreatedAt = DateTime.UtcNow;
    }
}
