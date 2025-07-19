using SistemaGestaoEscolar.Auth.Application.DTOs;

namespace SistemaGestaoEscolar.Auth.Application.Interfaces;

/// <summary>
/// Interface para serviços de aplicação de autenticação
/// </summary>
public interface IAuthApplicationService
{
    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    Task LogoutAsync(Guid userId);

    /// <summary>
    /// Renova o token de acesso usando refresh token
    /// </summary>
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);

    /// <summary>
    /// Solicita recuperação de senha
    /// </summary>
    Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

    /// <summary>
    /// Redefine a senha usando token de recuperação
    /// </summary>
    Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    /// <summary>
    /// Confirma o email do usuário
    /// </summary>
    Task ConfirmEmailAsync(string email, string token);

    /// <summary>
    /// Reenvia email de confirmação
    /// </summary>
    Task ResendEmailConfirmationAsync(string email);

    /// <summary>
    /// Invalida todas as sessões do usuário
    /// </summary>
    Task InvalidateAllSessionsAsync(Guid userId);

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    Task<UserInfoDto> GetCurrentUserAsync(Guid userId);

    /// <summary>
    /// Verifica se um email está disponível
    /// </summary>
    Task<bool> IsEmailAvailableAsync(string email);

    /// <summary>
    /// Valida a força de uma senha
    /// </summary>
    Task<PasswordStrengthDto> ValidatePasswordStrengthAsync(string password);
}

