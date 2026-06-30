using Microsoft.Extensions.Logging;
using Moq;
using MusicStreamer.Application.DTOs.Band;
using MusicStreamer.Application.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Tests;

public class BandServiceTests
{
    private readonly Mock<IBandRepository> _bandRepoMock;
    private readonly Mock<ILogger<BandService>> _loggerMock;
    private readonly BandService _bandService;

    public BandServiceTests()
    {
        _bandRepoMock = new Mock<IBandRepository>();
        _loggerMock = new Mock<ILogger<BandService>>();
        _bandService = new BandService(_bandRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBand()
    {
        // Arrange
        _bandRepoMock.Setup(r => r.AddAsync(It.IsAny<Band>(), default))
            .Returns(Task.CompletedTask);

        var request = new CreateBandRequest { Name = "Test Band", Description = "Desc" };

        // Act
        var result = await _bandService.CreateAsync(request);

        // Assert
        Assert.Equal("Test Band", result.Name);
        _bandRepoMock.Verify(r => r.AddAsync(It.IsAny<Band>(), default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _bandRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Band?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _bandService.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBands()
    {
        // Arrange
        var bands = new List<Band>
        {
            new Band("Band 1", "Desc 1"),
            new Band("Band 2", "Desc 2")
        };
        _bandRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(bands);

        // Act
        var result = await _bandService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var bands = new List<Band> { new Band("Metallica", "Heavy metal") };
        _bandRepoMock.Setup(r => r.SearchAsync("metal", 1, 10, default))
            .ReturnsAsync((bands, 1));

        // Act
        var result = await _bandService.SearchAsync("metal", 1, 10);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }
}
