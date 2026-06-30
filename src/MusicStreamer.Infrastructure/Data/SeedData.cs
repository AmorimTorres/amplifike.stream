using Microsoft.EntityFrameworkCore;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Enums;

namespace MusicStreamer.Infrastructure.Data;

public static class SeedData
{
    // Fixed GUIDs for reproducibility
    private static readonly Guid PlanFreeId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid PlanPremiumId = new("00000000-0000-0000-0000-000000000002");
    private static readonly Guid PlanFamilyId = new("00000000-0000-0000-0000-000000000003");

    private static readonly Guid AdminUserId = new("00000000-0000-0000-0000-000000000010");

    private static readonly Guid BandMetallicaId = new("00000000-0000-0000-0000-000000000101");
    private static readonly Guid BandPinkFloydId = new("00000000-0000-0000-0000-000000000102");
    private static readonly Guid BandQueenId = new("00000000-0000-0000-0000-000000000103");
    private static readonly Guid BandNirvanaId = new("00000000-0000-0000-0000-000000000104");

    private static readonly Guid AlbumBlackId = new("00000000-0000-0000-0000-000000000201");
    private static readonly Guid AlbumDivisionBellId = new("00000000-0000-0000-0000-000000000202");
    private static readonly Guid AlbumNightAtOperaId = new("00000000-0000-0000-0000-000000000203");
    private static readonly Guid AlbumNevermindId = new("00000000-0000-0000-0000-000000000204");

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedSubscriptionPlans(modelBuilder);
        SeedAdminUser(modelBuilder);
        SeedBands(modelBuilder);
        SeedAlbums(modelBuilder);
        SeedMusics(modelBuilder);
    }

    private static void SeedSubscriptionPlans(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubscriptionPlan>().HasData(
            CreatePlan(PlanFreeId, "Gratuito", 0.00m, 30),
            CreatePlan(PlanPremiumId, "Premium", 19.90m, 30),
            CreatePlan(PlanFamilyId, "Família", 29.90m, 30)
        );
    }

    private static object CreatePlan(Guid id, string name, decimal price, int duration)
    {
        // Use anonymous object for seed data (bypasses private setters)
        return new
        {
            Id = id,
            Name = name,
            Price = price,
            DurationInDays = duration,
            IsActive = true
        };
    }

    private static void SeedAdminUser(ModelBuilder modelBuilder)
    {
        // Password: Admin@123
        modelBuilder.Entity<User>().HasData(new
        {
            Id = AdminUserId,
            Name = "Administrador",
            Email = "admin@musicstreamer.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            AccountStatus = AccountStatus.Active,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            SubscriptionPlanId = (Guid?)null
        });
    }

    private static void SeedBands(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Band>().HasData(
            new { Id = BandMetallicaId, Name = "Metallica", Description = "Banda americana de heavy metal formada em 1981.", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = BandPinkFloydId, Name = "Pink Floyd", Description = "Banda britânica de rock progressivo formada em 1965.", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = BandQueenId, Name = "Queen", Description = "Banda britânica de rock formada em 1970.", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = BandNirvanaId, Name = "Nirvana", Description = "Banda americana de grunge formada em 1987.", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }

    private static void SeedAlbums(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>().HasData(
            new { Id = AlbumBlackId, Title = "Metallica (The Black Album)", ReleaseYear = 1991, BandId = BandMetallicaId },
            new { Id = AlbumDivisionBellId, Title = "The Division Bell", ReleaseYear = 1994, BandId = BandPinkFloydId },
            new { Id = AlbumNightAtOperaId, Title = "A Night at the Opera", ReleaseYear = 1975, BandId = BandQueenId },
            new { Id = AlbumNevermindId, Title = "Nevermind", ReleaseYear = 1991, BandId = BandNirvanaId }
        );
    }

    private static void SeedMusics(ModelBuilder modelBuilder)
    {
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Music>().HasData(
            // Metallica - Black Album
            new { Id = new Guid("00000000-0000-0000-0001-000000000001"), Title = "Enter Sandman", DurationInSeconds = 331, AlbumId = AlbumBlackId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0001-000000000002"), Title = "The Unforgiven", DurationInSeconds = 387, AlbumId = AlbumBlackId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0001-000000000003"), Title = "Nothing Else Matters", DurationInSeconds = 389, AlbumId = AlbumBlackId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0001-000000000004"), Title = "Sad But True", DurationInSeconds = 325, AlbumId = AlbumBlackId, CreatedAt = baseDate },

            // Pink Floyd - Division Bell
            new { Id = new Guid("00000000-0000-0000-0002-000000000001"), Title = "High Hopes", DurationInSeconds = 508, AlbumId = AlbumDivisionBellId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0002-000000000002"), Title = "Comfortably Numb", DurationInSeconds = 382, AlbumId = AlbumDivisionBellId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0002-000000000003"), Title = "Keep Talking", DurationInSeconds = 367, AlbumId = AlbumDivisionBellId, CreatedAt = baseDate },

            // Queen - Night at the Opera
            new { Id = new Guid("00000000-0000-0000-0003-000000000001"), Title = "Bohemian Rhapsody", DurationInSeconds = 355, AlbumId = AlbumNightAtOperaId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0003-000000000002"), Title = "You're My Best Friend", DurationInSeconds = 172, AlbumId = AlbumNightAtOperaId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0003-000000000003"), Title = "Love of My Life", DurationInSeconds = 218, AlbumId = AlbumNightAtOperaId, CreatedAt = baseDate },

            // Nirvana - Nevermind
            new { Id = new Guid("00000000-0000-0000-0004-000000000001"), Title = "Smells Like Teen Spirit", DurationInSeconds = 301, AlbumId = AlbumNevermindId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0004-000000000002"), Title = "Come as You Are", DurationInSeconds = 219, AlbumId = AlbumNevermindId, CreatedAt = baseDate },
            new { Id = new Guid("00000000-0000-0000-0004-000000000003"), Title = "Lithium", DurationInSeconds = 256, AlbumId = AlbumNevermindId, CreatedAt = baseDate }
        );
    }
}
