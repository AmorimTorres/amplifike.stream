using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Infrastructure.Data.Configurations;

namespace MusicStreamer.Infrastructure.Data;

public class MusicStreamerDbContext : DbContext
{
    public MusicStreamerDbContext(DbContextOptions<MusicStreamerDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Band> Bands => Set<Band>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Music> Musics => Set<Music>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistMusic> PlaylistMusics => Set<PlaylistMusic>();
    public DbSet<FavoriteMusic> FavoriteMusics => Set<FavoriteMusic>();
    public DbSet<FavoriteBand> FavoriteBands => Set<FavoriteBand>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MusicStreamerDbContext).Assembly);

        SeedData.Seed(modelBuilder);
    }
}
