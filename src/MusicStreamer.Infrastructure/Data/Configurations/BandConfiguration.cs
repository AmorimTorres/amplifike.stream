using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class BandConfiguration : IEntityTypeConfiguration<Band>
{
    public void Configure(EntityTypeBuilder<Band> builder)
    {
        builder.ToTable("Bands");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Index for search performance
        builder.HasIndex(b => b.Name);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);

        builder.Property(b => b.CreatedAt).IsRequired();

        builder.HasMany(b => b.Albums)
            .WithOne(a => a.Band)
            .HasForeignKey(a => a.BandId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
