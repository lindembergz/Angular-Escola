using SistemaGestaoEscolar.Auth.Domain.Entities;
using System.Security.Claims;

namespace SistemaGestaoEscolar.Auth.Application.Interfaces;

/// <summary>
/// Interface para serviços de geração e validação de tokens JWT
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um token de acesso JWT para o usuário
    /// </summary>
    Task<string> GenerateAccessTokenAsync(User user);

    /// <summary>
    /// Gera um refresh token para o usuário
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);

    /// <summary>
    /// Obtém claims do usuário para o token
    /// </summary>
    Task<IEnumerable<Claim>> GetUserClaimsAsync(User user);

    /// <summary>
    /// Obtém o tempo de expiração do token em segundos
    /// </summary>
    int GetTokenExpirationInSeconds();

    /// <summary>
    /// Obtém a data/hora de expiração do token
    /// </summary>
    DateTime GetTokenExpirationDateTime();

    /// <summary>
    /// Extrai o ID do usuário de um token
    /// </summary>
    Guid? ExtractUserIdFromToken(string token);

    /// <summary>
    /// Extrai o email do usuário de um token
    /// </summary>
    string? ExtractEmailFromToken(string token);

    /// <summary>
    /// Verifica se um token está expirado
    /// </summary>
    bool IsTokenExpired(string token);

    /// <summary>
    /// Gera um token de recuperação de senha
    /// </summary>
    string GeneratePasswordResetToken(User user);

    /// <summary>
    /// Valida um token de recuperação de senha
    /// </summary>
    bool ValidatePasswordResetToken(User user, string token);

    /// <summary>
    /// Gera um token de confirmação de email
    /// </summary>
    string GenerateEmailConfirmationToken(User user);

    /// <summary>
    /// Valida um token de confirmação de email
    /// </summary>
    bool ValidateEmailConfirmationToken(User user, string token);
}