using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Application.Services;

/// <summary>
/// Serviço de validação para operações de autenticação.
/// Centraliza validações complexas que envolvem múltiplas regras de negócio.
/// </summary>
public class AuthValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAuthDomainService _authDomainService;
    private readonly ILogger<AuthValidationService> _logger;

    public AuthValidationService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IAuthDomainService authDomainService,
        ILogger<AuthValidationService> logger)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _authDomainService = authDomainService;
        _logger = logger;
    }

    /// <summary>
    /// Valida dados de login antes da autenticação
    /// </summary>
    public async Task<ValidationResult> ValidateLoginAsync(LoginDto loginDto)
    {
        var errors = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(loginDto.Email))
            errors.Add("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(loginDto.Senha))
            errors.Add("Senha é obrigatória");

        if (errors.Any())
            return ValidationResult.Failure(errors);

        try
        {
            // Validar formato do email
            var email = new Email(loginDto.Email);

            // Verificar se há muitas tentativas de login do IP
            if (!string.IsNullOrWhiteSpace(loginDto.EnderecoIp))
            {
                var hasExcessiveAttempts = await _authDomainService.HasTooManyLoginAttemptsFromIpAsync(
                    loginDto.EnderecoIp, TimeSpan.FromMinutes(15));

                if (hasExcessiveAttempts)
                {
                    errors.Add("Muitas tentativas de login. Tente novamente em alguns minutos.");
                    return ValidationResult.Failure(errors);
                }

                // Verificar se o IP é suspeito
                var isSuspiciousIp = await _authDomainService.IsSuspiciousIpAsync(loginDto.EnderecoIp);
                if (isSuspiciousIp)
                {
                    _logger.LogWarning("Tentativa de login de IP suspeito: {IpAddress}", loginDto.EnderecoIp);
                    // Não bloquear, mas registrar para auditoria
                }
            }

            return ValidationResult.Success();
        }
        catch (ArgumentException ex)
        {
            errors.Add(ex.Message);
            return ValidationResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de login");
            errors.Add("Erro interno durante validação");
            return ValidationResult.Failure(errors);
        }
    }

    /// <summary>
    /// Valida dados de alteração de senha
    /// </summary>
    public ValidationResult ValidateChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
    {
        var errors = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(changePasswordDto.SenhaAtual))
            errors.Add("Senha atual é obrigatória");

        if (string.IsNullOrWhiteSpace(changePasswordDto.NovaSenha))
            errors.Add("Nova senha é obrigatória");

        if (changePasswordDto.NovaSenha != changePasswordDto.ConfirmarSenha)
            errors.Add("Nova senha e confirmação não conferem");

        if (errors.Any())
            return ValidationResult.Failure(errors);

        try
        {
            // Validar força da nova senha
            var passwordValidation = _passwordHashService.ValidatePassword(changePasswordDto.NovaSenha);
            if (!passwordValidation.IsValid)
            {
                errors.AddRange(passwordValidation.Errors);
            }

            // Verificar se a nova senha não é igual à atual
            if (changePasswordDto.SenhaAtual == changePasswordDto.NovaSenha)
            {
                errors.Add("A nova senha deve ser diferente da senha atual");
            }

            // Verificar se a senha não está comprometida
            var isCompromised = _passwordHashService.IsPasswordCompromised(changePasswordDto.NovaSenha);
            if (isCompromised)
            {
                errors.Add("Esta senha foi encontrada em vazamentos de dados. Escolha uma senha diferente.");
            }

            return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de alteração de senha");
            errors.Add("Erro interno durante validação");
            return ValidationResult.Failure(errors);
        }
    }

    /// <summary>
    /// Valida dados de reset de senha
    /// </summary>
    public ValidationResult ValidateResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var errors = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(resetPasswordDto.Email))
            errors.Add("Email é obrigatório");

        if (string.IsNullOrWhiteSpace(resetPasswordDto.Token))
            errors.Add("Token é obrigatório");

        if (string.IsNullOrWhiteSpace(resetPasswordDto.NovaSenha))
            errors.Add("Nova senha é obrigatória");

        if (resetPasswordDto.NovaSenha != resetPasswordDto.ConfirmarSenha)
            errors.Add("Nova senha e confirmação não conferem");

        if (errors.Any())
            return ValidationResult.Failure(errors);

        try
        {
            // Validar formato do email
            var email = new Email(resetPasswordDto.Email);

            // Validar força da nova senha
            var passwordValidation = _passwordHashService.ValidatePassword(resetPasswordDto.NovaSenha);
            if (!passwordValidation.IsValid)
            {
                errors.AddRange(passwordValidation.Errors);
            }

            // Verificar se a senha não está comprometida
            var isCompromised = _passwordHashService.IsPasswordCompromised(resetPasswordDto.NovaSenha);
            if (isCompromised)
            {
                errors.Add("Esta senha foi encontrada em vazamentos de dados. Escolha uma senha diferente.");
            }

            return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
        }
        catch (ArgumentException ex)
        {
            errors.Add(ex.Message);
            return ValidationResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de reset de senha");
            errors.Add("Erro interno durante validação");
            return ValidationResult.Failure(errors);
        }
    }

    /// <summary>
    /// Valida se um email pode ser usado para criar conta
    /// </summary>
    public async Task<ValidationResult> ValidateEmailForRegistrationAsync(string email)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add("Email é obrigatório");
            return ValidationResult.Failure(errors);
        }

        try
        {
            // Validar formato do email
            var emailObj = new Email(email);

            // Verificar se o email já está em uso
            var emailExists = await _userRepository.ExistePorEmailAsync(emailObj);
            if (emailExists)
            {
                errors.Add("Este email já está em uso");
            }

            // Verificar se o email pode ser usado para criar conta
            var canCreateAccount = await _authDomainService.CanCreateAccountWithEmailAsync(emailObj);
            if (!canCreateAccount)
            {
                errors.Add("Este email não pode ser usado para criar uma conta");
            }

            return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
        }
        catch (ArgumentException ex)
        {
            errors.Add(ex.Message);
            return ValidationResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de email para registro");
            errors.Add("Erro interno durante validação");
            return ValidationResult.Failure(errors);
        }
    }

    /// <summary>
    /// Valida dados de refresh token
    /// </summary>
    public ValidationResult ValidateRefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
            errors.Add("Refresh token é obrigatório");

        return errors.Any() ? ValidationResult.Failure(errors) : ValidationResult.Success();
    }
}

/// <summary>
/// Resultado de validação
/// </summary>
public record ValidationResult(
    bool IsValid,
    IEnumerable<string> Errors
)
{
    public static ValidationResult Success() => new(true, Array.Empty<string>());
    public static ValidationResult Failure(IEnumerable<string> errors) => new(false, errors);
    public static ValidationResult Failure(string error) => new(false, new[] { error });
}