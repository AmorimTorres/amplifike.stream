using Microsoft.Extensions.DependencyInjection;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Application.Services;

namespace MusicStreamer.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBandService, BandService>();
        services.AddScoped<IAlbumService, AlbumService>();
        services.AddScoped<IMusicService, MusicService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}
