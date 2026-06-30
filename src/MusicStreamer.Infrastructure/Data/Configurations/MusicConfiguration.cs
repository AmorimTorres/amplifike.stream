using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class MusicConfiguration : IEntityTypeConfiguration<Music>
{
    public void Configure(EntityTypeBuilder<Music> builder)
    {
        builder.ToTable("Musics");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(300);

        // Index for search performance
        builder.HasIndex(m => m.Title);

        builder.Property(m => m.DurationInSeconds).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}
