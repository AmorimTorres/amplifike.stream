namespace MusicStreamer.Domain.Entities;

public class Playlist
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User? User { get; private set; }
    public ICollection<PlaylistMusic> PlaylistMusics { get; private set; } = new List<PlaylistMusic>();

    private Playlist() { }

    public Playlist(string name, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = name;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        Name = name;
    }
}
