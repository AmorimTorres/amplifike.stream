using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreamer.Domain.Entities;

namespace MusicStreamer.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Merchant)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.RequestedAt).IsRequired();
        builder.Property(t => t.AuthorizedAt).IsRequired(false);
        builder.Property(t => t.Status).IsRequired();

        builder.Property(t => t.DenialReason)
            .HasMaxLength(500);

        builder.HasIndex(t => new { t.UserId, t.RequestedAt });

        builder.HasMany(t => t.Notifications)
            .WithOne(n => n.Transaction)
            .HasForeignKey(n => n.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
