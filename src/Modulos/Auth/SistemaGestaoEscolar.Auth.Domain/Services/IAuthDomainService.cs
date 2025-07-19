using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Domain.Services;

/// <summary>
/// Interface para serviços de domínio de autenticação.
/// Contém lógica de negócio que não pertence a uma entidade específica.
/// </summary>
public interface IAuthDomainService
{
    /// <summary>
    /// Valida se um usuário pode fazer login
    /// </summary>
    Task<LoginValidationResult> ValidateLoginAsync(User user, string password, string ipAddress, string userAgent);

    /// <summary>
    /// Valida se um email pode ser usado para criar uma nova conta
    /// </summary>
    Task<bool> CanCreateAccountWithEmailAsync(Email email);

    /// <summary>
    /// Valida se um usuário pode alterar seu papel
    /// </summary>
    bool CanChangeRole(User currentUser, User targetUser, UserRole newRole);

    /// <summary>
    /// Valida se um usuário pode gerenciar outro usuário
    /// </summary>
    bool PodeGerenciarUsuario(User manager, User? targetUser);

    /// <summary>
    /// Valida se um usuário pode acessar uma escola
    /// </summary>
    bool CanAccessSchool(User user, Guid schoolId);

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

    /// <summary>
    /// Verifica se um IP está na lista de IPs suspeitos
    /// </summary>
    Task<bool> IsSuspiciousIpAsync(string ipAddress);

    /// <summary>
    /// Verifica se há muitas tentativas de login de um IP
    /// </summary>
    Task<bool> HasTooManyLoginAttemptsFromIpAsync(string ipAddress, TimeSpan timeWindow);

    /// <summary>
    /// Registra uma tentativa de login suspeita
    /// </summary>
    Task RegisterSuspiciousLoginAttemptAsync(string ipAddress, string userAgent, string email);

    /// <summary>
    /// Limpa dados de segurança antigos
    /// </summary>
    void CleanupSecurityData();

    /// <summary>
    /// Obtém recomendações de segurança para um usuário
    /// </summary>
    Task<IEnumerable<SecurityRecommendation>> GetSecurityRecommendationsAsync(User user);

    /// <summary>
    /// Calcula o nível de risco de uma sessão
    /// </summary>
    Task<RiskLevel> CalculateSessionRiskAsync(Session session);

    /// <summary>
    /// Verifica se um usuário deve ser forçado a alterar a senha
    /// </summary>
    bool ShouldForcePasswordChange(User user);

    /// <summary>
    /// Verifica se um usuário deve ser desativado por inatividade
    /// </summary>
    bool ShouldDeactivateForInactivity(User user);
}

/// <summary>
/// Resultado da validação de login
/// </summary>
public record LoginValidationResult(
    bool IsValid,
    string? ErrorMessage = null,
    bool RequiresPasswordChange = false,
    bool RequiresEmailConfirmation = false,
    bool IsSuspicious = false,
    TimeSpan? LockoutDuration = null
);

/// <summary>
/// Recomendação de segurança
/// </summary>
public record SecurityRecommendation(
    string Title,
    string Description,
    SecurityRecommendationType Type,
    SecurityRecommendationPriority Priority
);

/// <summary>
/// Tipo de recomendação de segurança
/// </summary>
public enum SecurityRecommendationType
{
    PasswordChange,
    EnableTwoFactor,
    ReviewSessions,
    UpdateContactInfo,
    SecurityAudit
}

/// <summary>
/// Prioridade da recomendação de segurança
/// </summary>
public enum SecurityRecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Nível de risco
/// </summary>
public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}