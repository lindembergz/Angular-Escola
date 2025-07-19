using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para solicitação de recuperação de senha.
/// Gera token de recuperação e envia por email.
/// </summary>
public class ForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IAuthDomainService _authDomainService;
    private readonly ILogger<ForgotPasswordUseCase> _logger;

    public ForgotPasswordUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        IAuthDomainService authDomainService,
        ILogger<ForgotPasswordUseCase> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _authDomainService = authDomainService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de recuperação de senha
    /// </summary>
    public async Task ExecuteAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            _logger.LogInformation("Solicitação de recuperação de senha para email: {Email}", forgotPasswordDto.Email);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
            {
                _logger.LogWarning("Tentativa de recuperação com email vazio");
                return; // Não revelar erro por segurança
            }

            // 2. Buscar usuário por email
            var user = await _userRepository.ObterPorEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                _logger.LogInformation("Email não encontrado para recuperação: {Email}", forgotPasswordDto.Email);
                // Não revelar se o email existe ou não por segurança
                return;
            }

            // 3. Verificar se o usuário está ativo
            if (!user.Ativo)
            {
                _logger.LogWarning("Tentativa de recuperação para usuário inativo: {UserId}", user.Id);
                return; // Não revelar status do usuário
            }

            // 4. Verificar se a conta não está bloqueada
            if (user.EstaBloqueada())
            {
                _logger.LogWarning("Tentativa de recuperação para usuário bloqueado: {UserId}", user.Id);
                return; // Não revelar status do usuário
            }

            // 5. Verificar se não há muitas tentativas de recuperação
            var hasExcessiveAttempts = await _authDomainService.HasTooManyLoginAttemptsFromIpAsync(
                "password-reset", TimeSpan.FromHours(1));
            
            if (hasExcessiveAttempts)
            {
                _logger.LogWarning("Muitas tentativas de recuperação de senha para email: {Email}", forgotPasswordDto.Email);
                return; // Não revelar limitação por segurança
            }

            // 6. Gerar token de recuperação
            var resetToken = _tokenService.GeneratePasswordResetToken(user);

            // 7. TODO: Enviar email com token de recuperação
            // Por enquanto, apenas log para desenvolvimento
            _logger.LogInformation("Token de recuperação gerado para usuário: {UserId}", user.Id);
            _logger.LogDebug("Token de recuperação: {Token} (CallbackUrl: {CallbackUrl})", 
                resetToken, forgotPasswordDto.UrlCallback);

            // 8. Registrar tentativa de recuperação para auditoria
            await _authDomainService.RegisterSuspiciousLoginAttemptAsync(
                "password-reset", "forgot-password", forgotPasswordDto.Email);

            _logger.LogInformation("Processo de recuperação de senha iniciado para usuário: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante solicitação de recuperação de senha para email: {Email}", 
                forgotPasswordDto.Email);
            // Não propagar erro para não revelar informações sensíveis
        }
    }
}