using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MusicStreamer.Application.DTOs.Auth;
using MusicStreamer.Application.Interfaces.Services;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Exceptions;
using MusicStreamer.Domain.Interfaces.Repositories;

namespace MusicStreamer.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registrando novo usuário: {Email}", request.Email);

        var emailNormalized = request.Email.ToLower().Trim();

        if (await _userRepository.ExistsByEmailAsync(emailNormalized, cancellationToken))
            throw new ConflictException($"E-mail '{request.Email}' já está em uso.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, emailNormalized, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        _logger.LogInformation("Usuário registrado com sucesso: {UserId}", user.Id);

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tentativa de login: {Email}", request.Email);

        var emailNormalized = request.Email.ToLower().Trim();
        var user = await _userRepository.GetByEmailAsync(emailNormalized, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("E-mail ou senha inválidos.");

        if (!user.IsActive())
            throw new UnauthorizedException("Conta bloqueada ou suspensa. Entre em contato com o suporte.");

        _logger.LogInformation("Login realizado com sucesso: {UserId}", user.Id);

        return GenerateAuthResponse(user);
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário", userId);

        return new UserProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.AccountStatus.ToString(),
            user.CreatedAt
        );
    }

    private AuthResponse GenerateAuthResponse(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
        var expiresAt = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiresInHours"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponse(
            Token: tokenHandler.WriteToken(token),
            Name: user.Name,
            Email: user.Email,
            Role: user.Role,
            ExpiresAt: expiresAt
        );
    }
}
