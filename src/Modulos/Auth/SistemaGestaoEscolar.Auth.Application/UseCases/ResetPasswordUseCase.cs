using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para redefinição de senha usando token de recuperação.
/// Valida token e redefine senha com segurança.
/// </summary>
public class ResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<ResetPasswordUseCase> _logger;

    public ResetPasswordUseCase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        ITokenService tokenService,
        IPasswordHashService passwordHashService,
        ILogger<ResetPasswordUseCase> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de redefinição de senha
    /// </summary>
    public async Task ExecuteAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            _logger.LogInformation("Tentativa de reset de senha para email: {Email}", resetPasswordDto.Email);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(resetPasswordDto.Email) ||
                string.IsNullOrWhiteSpace(resetPasswordDto.Token) ||
                string.IsNullOrWhiteSpace(resetPasswordDto.NovaSenha))
            {
                throw new ArgumentException("Email, token e nova senha são obrigatórios");
            }

            if (resetPasswordDto.NovaSenha != resetPasswordDto.ConfirmarSenha)
            {
                throw new ArgumentException("Nova senha e confirmação não conferem");
            }

            // 2. Buscar usuário por email
            var user = await _userRepository.ObterPorEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Email não encontrado para reset: {Email}", resetPasswordDto.Email);
                throw new InvalidOperationException("Token inválido ou expirado");
            }

            // 3. Validar token de reset
            if (!_tokenService.ValidatePasswordResetToken(user, resetPasswordDto.Token))
            {
                _logger.LogWarning("Token de reset inválido para usuário: {UserId}", user.Id);
                
                // Registrar tentativa suspeita
                user.RegistrarLoginFalhado();
                _userRepository.Atualizar(user);
                await _userRepository.SalvarAlteracoesAsync();
                
                throw new InvalidOperationException("Token inválido ou expirado");
            }

            // 4. Validar nova senha
            var passwordValidation = _passwordHashService.ValidatePassword(resetPasswordDto.NovaSenha);
            if (!passwordValidation.IsValid)
            {
                var errors = string.Join(", ", passwordValidation.Errors);
                _logger.LogWarning("Nova senha inválida no reset para usuário {UserId}: {Errors}", user.Id, errors);
                throw new ArgumentException($"Nova senha inválida: {errors}");
            }

            // 5. Verificar se a nova senha é diferente da atual (se possível verificar)
            if (user.VerificarSenha(resetPasswordDto.NovaSenha))
            {
                _logger.LogWarning("Tentativa de usar a mesma senha atual no reset para usuário: {UserId}", user.Id);
                throw new ArgumentException("A nova senha deve ser diferente da senha atual");
            }

            // 6. Alterar senha
            var newPassword = new Password(resetPasswordDto.NovaSenha);
            user.AlterarSenha(newPassword);

            // 7. Ativar usuário se estava inativo
            if (!user.Ativo)
            {
                user.Ativar();
                _logger.LogInformation("Usuário reativado durante reset de senha: {UserId}", user.Id);
            }

            // 8. Desbloquear se estava bloqueado
            if (user.EstaBloqueada())
            {
                user.Desbloquear();
                _logger.LogInformation("Usuário desbloqueado durante reset de senha: {UserId}", user.Id);
            }

            // 9. Invalidar todas as sessões ativas por segurança
            await _sessionRepository.InvalidarTodasSessoesUsuarioAsync(user.Id);

            // 10. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();
            await _sessionRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Senha redefinida com sucesso para usuário: {UserId}", user.Id);
        }
        catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException)
        {
            _logger.LogError(ex, "Erro inesperado durante reset de senha para email: {Email}", resetPasswordDto.Email);
            throw new InvalidOperationException("Erro interno durante reset de senha");
        }
    }
}