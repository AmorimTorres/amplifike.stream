using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class FavoriteMusicConfiguration : IEntityTypeConfiguration<FavoriteMusic>
{
    public void Configure(EntityTypeBuilder<FavoriteMusic> builder)
    {
        builder.ToTable("FavoriteMusics");

        // Composite primary key
        builder.HasKey(f => new { f.UserId, f.MusicId });

        builder.Property(f => f.CreatedAt).IsRequired();

        builder.HasOne(f => f.Music)
            .WithMany(m => m.FavoriteMusics)
            .HasForeignKey(f => f.MusicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
