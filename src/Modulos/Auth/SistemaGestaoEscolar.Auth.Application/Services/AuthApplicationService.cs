using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Application.UseCases;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;


namespace SistemaGestaoEscolar.Auth.Application.Services;

/// <summary>
/// Serviço de aplicação para operações de autenticação.
/// Orquestra casos de uso e coordena operações complexas.
/// </summary>
public class AuthApplicationService : IAuthApplicationService
{
    private readonly LoginUseCase _loginUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly ChangePasswordUseCase _changePasswordUseCase;
    private readonly ForgotPasswordUseCase _forgotPasswordUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;
    private readonly ConfirmEmailUseCase _confirmEmailUseCase;
    private readonly ResendEmailConfirmationUseCase _resendEmailConfirmationUseCase;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<AuthApplicationService> _logger;

    public AuthApplicationService(
        LoginUseCase loginUseCase,
        LogoutUseCase logoutUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        ChangePasswordUseCase changePasswordUseCase,
        ForgotPasswordUseCase forgotPasswordUseCase,
        ResetPasswordUseCase resetPasswordUseCase,
        ConfirmEmailUseCase confirmEmailUseCase,
        ResendEmailConfirmationUseCase resendEmailConfirmationUseCase,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ILogger<AuthApplicationService> logger)
    {
        _loginUseCase = loginUseCase;
        _logoutUseCase = logoutUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _changePasswordUseCase = changePasswordUseCase;
        _forgotPasswordUseCase = forgotPasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _confirmEmailUseCase = confirmEmailUseCase;
        _resendEmailConfirmationUseCase = resendEmailConfirmationUseCase;
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        return await _loginUseCase.ExecuteAsync(loginDto);
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    public async Task LogoutAsync(Guid userId)
    {
        await _logoutUseCase.ExecuteAsync(userId);
    }

    /// <summary>
    /// Renova o token de acesso usando refresh token
    /// </summary>
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        return await _refreshTokenUseCase.ExecuteAsync(refreshTokenDto);
    }

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        await _changePasswordUseCase.ExecuteAsync(userId, changePasswordDto);
    }

    /// <summary>
    /// Solicita recuperação de senha
    /// </summary>
    public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        await _forgotPasswordUseCase.ExecuteAsync(forgotPasswordDto);
    }

    /// <summary>
    /// Redefine a senha usando token de recuperação
    /// </summary>
    public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        await _resetPasswordUseCase.ExecuteAsync(resetPasswordDto);
    }

    /// <summary>
    /// Confirma o email do usuário
    /// </summary>
    public async Task ConfirmEmailAsync(string email, string token)
    {
        await _confirmEmailUseCase.ExecuteAsync(email, token);
    }

    /// <summary>
    /// Reenvia email de confirmação
    /// </summary>
    public async Task ResendEmailConfirmationAsync(string email)
    {
        await _resendEmailConfirmationUseCase.ExecuteAsync(email);
    }

    /// <summary>
    /// Invalida todas as sessões do usuário
    /// </summary>
    public async Task InvalidateAllSessionsAsync(Guid userId)
    {
        await _logoutUseCase.ExecuteAsync(userId);
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    public async Task<UserInfoDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.ObterPorIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        return new UserInfoDto
        {
            Id = user.Id,
            PrimeiroNome = user.PrimeiroNome,
            UltimoNome = user.UltimoNome,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email.Value,
            CodigoPerfil = user.Perfil.Code,
            NomePerfil = user.Perfil.Name,
            NivelPerfil = user.Perfil.Level,
            Iniciais = user.Iniciais,
            Ativo = user.Ativo,
            EmailConfirmado = user.EmailConfirmado,
            UltimoLoginEm = user.UltimoLoginEm,
            EscolaId = user.EscolaId
        };
    }

    /// <summary>
    /// Verifica se um email está disponível
    /// </summary>
    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        try
        {
            return !await _userRepository.ExistePorEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade do email: {Email}", email);
            return false; // Assumir não disponível em caso de erro
        }
    }

    /// <summary>
    /// Valida a força de uma senha
    /// </summary>
    public Task<PasswordStrengthDto> ValidatePasswordStrengthAsync(string password)
    {
        var forca = Password.CalcularForca(password);

        var nivel = forca switch
        {
            < 20 => NivelForcaSenha.MuitoFraca,
            < 40 => NivelForcaSenha.Fraca,
            < 60 => NivelForcaSenha.Razoavel,
            < 80 => NivelForcaSenha.Boa,
            < 95 => NivelForcaSenha.Forte,
            _ => NivelForcaSenha.MuitoForte
        };

        var erros = new List<string>();
        var sugestoes = new List<string>();
        var ehValida = true;

        // Validações básicas
        if (password.Length < 8)
        {
            erros.Add("Senha deve ter pelo menos 8 caracteres");
            sugestoes.Add("Use pelo menos 8 caracteres");
            ehValida = false;
        }

        if (!password.Any(char.IsUpper))
        {
            erros.Add("Senha deve conter pelo menos uma letra maiúscula");
            sugestoes.Add("Adicione pelo menos uma letra maiúscula");
            ehValida = false;
        }

        if (!password.Any(char.IsLower))
        {
            erros.Add("Senha deve conter pelo menos uma letra minúscula");
            sugestoes.Add("Adicione pelo menos uma letra minúscula");
            ehValida = false;
        }

        if (!password.Any(char.IsDigit))
        {
            erros.Add("Senha deve conter pelo menos um número");
            sugestoes.Add("Adicione pelo menos um número");
            ehValida = false;
        }

        return Task.FromResult(new PasswordStrengthDto
        {
            Forca = forca,
            EhValida = ehValida,
            Erros = erros,
            Sugestoes = sugestoes,
            Nivel = nivel
        });
    }
}