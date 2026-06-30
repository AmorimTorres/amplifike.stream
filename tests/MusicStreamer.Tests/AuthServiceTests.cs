using Microsoft.Extensions.Logging;
using Moq;
using MusicStreamer.Application.DTOs.Auth;
using MusicStreamer.Application.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace MusicStreamer.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        // Setup JWT configuration
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(s => s["Key"]).Returns("TestSuperSecretKey_AtLeast32Chars!!");
        jwtSection.Setup(s => s["Issuer"]).Returns("TestIssuer");
        jwtSection.Setup(s => s["Audience"]).Returns("TestAudience");
        jwtSection.Setup(s => s["ExpiresInHours"]).Returns("24");
        _configMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSection.Object);

        _authService = new AuthService(_userRepoMock.Object, _configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldReturnToken()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("user@test.com", default))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), default))
            .Returns(Task.CompletedTask);

        var request = new RegisterRequest
        {
            Name = "Test User",
            Email = "user@test.com",
            Password = "Password123"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("user@test.com", result.Email);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowConflictException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("existing@test.com", default))
            .ReturnsAsync(true);

        var request = new RegisterRequest
        {
            Name = "Test",
            Email = "existing@test.com",
            Password = "Password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldThrowUnauthorizedException()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync("wrong@test.com", default))
            .ReturnsAsync((User?)null);

        var request = new LoginRequest { Email = "wrong@test.com", Password = "wrong" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));
    }
}
