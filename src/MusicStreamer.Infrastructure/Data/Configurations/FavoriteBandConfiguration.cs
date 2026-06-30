using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class FavoriteBandConfiguration : IEntityTypeConfiguration<FavoriteBand>
{
    public void Configure(EntityTypeBuilder<FavoriteBand> builder)
    {
        builder.ToTable("FavoriteBands");

        // Composite primary key
        builder.HasKey(f => new { f.UserId, f.BandId });

        builder.Property(f => f.CreatedAt).IsRequired();

        builder.HasOne(f => f.Band)
            .WithMany(b => b.FavoriteBands)
            .HasForeignKey(f => f.BandId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
