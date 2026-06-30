namespace MusicStreamer.Domain.Entities;

public class Music
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int DurationInSeconds { get; private set; }
    public Guid AlbumId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public Album? Album { get; private set; }
    public ICollection<PlaylistMusic> PlaylistMusics { get; private set; } = new List<PlaylistMusic>();
    public ICollection<FavoriteMusic> FavoriteMusics { get; private set; } = new List<FavoriteMusic>();

    private Music() { }

    public Music(string title, int durationInSeconds, Guid albumId)
    {
        Id = Guid.NewGuid();
        Title = title;
        DurationInSeconds = durationInSeconds;
        AlbumId = albumId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, int durationInSeconds)
    {
        Title = title;
        DurationInSeconds = durationInSeconds;
    }
}
