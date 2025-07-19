using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para alteração de senha do usuário.
/// Implementa validações de segurança e invalidação de sessões.
/// </summary>
public class ChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<ChangePasswordUseCase> _logger;

    public ChangePasswordUseCase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHashService passwordHashService,
        ILogger<ChangePasswordUseCase> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de alteração de senha
    /// </summary>
    public async Task ExecuteAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            _logger.LogInformation("Iniciando alteração de senha para usuário: {UserId}", userId);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(changePasswordDto.SenhaAtual) ||
                string.IsNullOrWhiteSpace(changePasswordDto.NovaSenha))
            {
                throw new ArgumentException("Senha atual e nova senha são obrigatórias");
            }

            if (changePasswordDto.NovaSenha != changePasswordDto.ConfirmarSenha)
            {
                throw new ArgumentException("Nova senha e confirmação não conferem");
            }

            // 2. Buscar usuário
            var user = await _userRepository.ObterPorIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para alteração de senha: {UserId}", userId);
                throw new InvalidOperationException("Usuário não encontrado");
            }

            // 3. Verificar se o usuário está ativo
            if (!user.Ativo)
            {
                _logger.LogWarning("Tentativa de alteração de senha para usuário inativo: {UserId}", userId);
                throw new UnauthorizedAccessException("Usuário inativo");
            }

            // 4. Verificar senha atual
            if (!user.VerificarSenha(changePasswordDto.SenhaAtual))
            {
                _logger.LogWarning("Senha atual incorreta para usuário: {UserId}", userId);
                user.RegistrarLoginFalhado(); // Registrar tentativa suspeita
                _userRepository.Atualizar(user);
                await _userRepository.SalvarAlteracoesAsync();
                throw new UnauthorizedAccessException("Senha atual incorreta");
            }

            // 5. Validar nova senha
            var passwordValidation = _passwordHashService.ValidatePassword(changePasswordDto.NovaSenha);
            if (!passwordValidation.IsValid)
            {
                var errors = string.Join(", ", passwordValidation.Errors);
                _logger.LogWarning("Nova senha inválida para usuário {UserId}: {Errors}", userId, errors);
                throw new ArgumentException($"Nova senha inválida: {errors}");
            }

            // 6. Verificar se a nova senha é diferente da atual
            if (user.VerificarSenha(changePasswordDto.NovaSenha))
            {
                _logger.LogWarning("Tentativa de usar a mesma senha atual para usuário: {UserId}", userId);
                throw new ArgumentException("A nova senha deve ser diferente da senha atual");
            }

            // 7. Alterar senha
            var newPassword = new Password(changePasswordDto.NovaSenha);
            user.AlterarSenha(newPassword);

            // 8. Invalidar sessões se solicitado
            if (changePasswordDto.InvalidarTodasSessoes)
            {
                await _sessionRepository.InvalidarTodasSessoesUsuarioAsync(userId);
                _logger.LogInformation("Todas as sessões invalidadas para usuário: {UserId}", userId);
            }

            // 9. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();
            await _sessionRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException and not ArgumentException and not InvalidOperationException)
        {
            _logger.LogError(ex, "Erro inesperado durante alteração de senha para usuário: {UserId}", userId);
            throw new InvalidOperationException("Erro interno durante alteração de senha");
        }
    }
}