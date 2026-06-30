using System.ComponentModel.DataAnnotations;

namespace MusicStreamer.Application.DTOs.Auth;

public record RegisterRequest
{
    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(300)]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres.")]
    public string Password { get; init; } = string.Empty;
}

public record LoginRequest
{
    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória.")]
    public string Password { get; init; } = string.Empty;
}

public record AuthResponse(
    string Token,
    string Name,
    string Email,
    string Role,
    DateTime ExpiresAt
);

public record UserProfileResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string AccountStatus,
    DateTime CreatedAt
);
