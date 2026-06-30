namespace MusicStreamer.Domain.Entities;

public class FavoriteBand
{
    public Guid UserId { get; private set; }
    public Guid BandId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation
    public User? User { get; private set; }
    public Band? Band { get; private set; }

    private FavoriteBand() { }

    public FavoriteBand(Guid userId, Guid bandId)
    {
        UserId = userId;
        BandId = bandId;
        CreatedAt = DateTime.UtcNow;
    }
}
