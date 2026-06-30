using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Infrastructure.Data;
using MusicStreamer.Infrastructure.Repositories;
using MusicStreamer.Infrastructure.Services;

namespace MusicStreamer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<MusicStreamerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MusicStreamerDbContext).Assembly.FullName)
            ));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IMusicRepository, MusicRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Services
        services.AddScoped<INotificationService, MockNotificationService>();

        return services;
    }
}
