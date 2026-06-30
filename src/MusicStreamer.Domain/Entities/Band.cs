namespace MusicStreamer.Domain.Entities;

public class Band
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public ICollection<Album> Albums { get; private set; } = new List<Album>();
    public ICollection<FavoriteBand> FavoriteBands { get; private set; } = new List<FavoriteBand>();

    private Band() { }

    public Band(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}
