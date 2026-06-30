using Microsoft.Extensions.Logging;
using Moq;
using MusicStreamer.Application.DTOs.Playlist;
using MusicStreamer.Application.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Tests;

public class PlaylistServiceTests
{
    private readonly Mock<IPlaylistRepository> _playlistRepoMock;
    private readonly Mock<IMusicRepository> _musicRepoMock;
    private readonly Mock<ILogger<PlaylistService>> _loggerMock;
    private readonly PlaylistService _playlistService;

    public PlaylistServiceTests()
    {
        _playlistRepoMock = new Mock<IPlaylistRepository>();
        _musicRepoMock = new Mock<IMusicRepository>();
        _loggerMock = new Mock<ILogger<PlaylistService>>();
        _playlistService = new PlaylistService(_playlistRepoMock.Object, _musicRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePlaylist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _playlistRepoMock.Setup(r => r.AddAsync(It.IsAny<Playlist>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _playlistService.CreateAsync(userId, new CreatePlaylistRequest { Name = "Minha Playlist" });

        // Assert
        Assert.Equal("Minha Playlist", result.Name);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task AddMusicAsync_WhenPlaylistBelongsToOtherUser_ShouldThrowForbiddenException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var playlist = new Playlist("Test", ownerId);

        _playlistRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(playlist);
        _musicRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _playlistService.AddMusicAsync(playlist.Id, Guid.NewGuid(), otherUserId));
    }

    [Fact]
    public async Task AddMusicAsync_WhenMusicAlreadyInPlaylist_ShouldThrowConflictException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var musicId = Guid.NewGuid();
        var playlist = new Playlist("Test", userId);

        _playlistRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(playlist);
        _musicRepoMock.Setup(r => r.ExistsAsync(musicId, default))
            .ReturnsAsync(true);
        _playlistRepoMock.Setup(r => r.GetPlaylistMusicAsync(It.IsAny<Guid>(), musicId, default))
            .ReturnsAsync(new PlaylistMusic(playlistId, musicId));

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            _playlistService.AddMusicAsync(playlist.Id, musicId, userId));
    }
}
