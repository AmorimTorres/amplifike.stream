namespace MusicStreamer.Domain.Entities;

public class Album
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int ReleaseYear { get; private set; }
    public Guid BandId { get; private set; }

    // Navigation
    public Band? Band { get; private set; }
    public ICollection<Music> Musics { get; private set; } = new List<Music>();

    private Album() { }

    public Album(string title, int releaseYear, Guid bandId)
    {
        Id = Guid.NewGuid();
        Title = title;
        ReleaseYear = releaseYear;
        BandId = bandId;
    }

    public void Update(string title, int releaseYear)
    {
        Title = title;
        ReleaseYear = releaseYear;
    }
}
