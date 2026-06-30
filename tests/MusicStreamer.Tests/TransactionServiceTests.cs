using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MusicStreamer.Application.DTOs.Transaction;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Application.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Enums;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Tests;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<INotificationService> _notificationMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<TransactionService>> _loggerMock;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _transactionRepoMock = new Mock<ITransactionRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _notificationMock = new Mock<INotificationService>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<TransactionService>>();

        _service = new TransactionService(
            _transactionRepoMock.Object,
            _userRepoMock.Object,
            _notificationMock.Object,
            _configMock.Object,
            _loggerMock.Object
        );
    }

    private User CreateActiveUser()
    {
        var user = new User("Test User", "test@test.com", "hash");
        return user;
    }

    [Fact]
    public async Task AuthorizeAsync_WithValidRequest_ShouldAuthorizeTransaction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateActiveUser();
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _transactionRepoMock.Setup(r => r.GetLastTransactionByUserIdAsync(userId, default))
            .ReturnsAsync((Transaction?)null);
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);
        _notificationMock.Setup(n => n.SendToUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);
        _notificationMock.Setup(n => n.SendToMerchantAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

        var request = new AuthorizeTransactionRequest
        {
            Merchant = "Spotify",
            Amount = 19.90m,
            RequestedAt = DateTime.UtcNow
        };

        // Act
        var result = await _service.AuthorizeAsync(userId, request);

        // Assert
        Assert.Equal("Authorized", result.Status);
        Assert.Null(result.DenialReason);
    }

    [Fact]
    public async Task AuthorizeAsync_WithAmountExceedingLimit_ShouldDenyTransaction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateActiveUser();
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _transactionRepoMock.Setup(r => r.GetLastTransactionByUserIdAsync(userId, default))
            .ReturnsAsync((Transaction?)null);
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);

        var request = new AuthorizeTransactionRequest
        {
            Merchant = "Merchant",
            Amount = 99999m, // exceeds limit
            RequestedAt = DateTime.UtcNow
        };

        // Act
        var result = await _service.AuthorizeAsync(userId, request);

        // Assert
        Assert.Equal("Denied", result.Status);
        Assert.NotNull(result.DenialReason);
    }

    [Fact]
    public async Task AuthorizeAsync_WithTooRecentTransaction_ShouldDenyAsDuplicate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = CreateActiveUser();
        var lastTransaction = new Transaction(userId, "Merchant", 10m, DateTime.UtcNow.AddSeconds(-30));

        _userRepoMock.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync(user);
        _transactionRepoMock.Setup(r => r.GetLastTransactionByUserIdAsync(userId, default))
            .ReturnsAsync(lastTransaction);
        _transactionRepoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);

        var request = new AuthorizeTransactionRequest
        {
            Merchant = "Other Merchant",
            Amount = 10m,
            RequestedAt = DateTime.UtcNow // 30 seconds after last - less than 1 minute
        };

        // Act
        var result = await _service.AuthorizeAsync(userId, request);

        // Assert
        Assert.Equal("Denied", result.Status);
        Assert.Contains("Intervalo mínimo", result.DenialReason);
    }
}
