using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class PlaylistMusicConfiguration : IEntityTypeConfiguration<PlaylistMusic>
{
    public void Configure(EntityTypeBuilder<PlaylistMusic> builder)
    {
        builder.ToTable("PlaylistMusics");

        // Composite primary key
        builder.HasKey(pm => new { pm.PlaylistId, pm.MusicId });

        builder.Property(pm => pm.AddedAt).IsRequired();

        builder.HasOne(pm => pm.Playlist)
            .WithMany(p => p.PlaylistMusics)
            .HasForeignKey(pm => pm.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.Music)
            .WithMany(m => m.PlaylistMusics)
            .HasForeignKey(pm => pm.MusicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
