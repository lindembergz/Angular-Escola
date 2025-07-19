using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Repositories;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para confirmação de email do usuário.
/// Valida token de confirmação e ativa o email.
/// </summary>
public class ConfirmEmailUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ConfirmEmailUseCase> _logger;

    public ConfirmEmailUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<ConfirmEmailUseCase> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de confirmação de email
    /// </summary>
    public async Task ExecuteAsync(string email, string token)
    {
        try
        {
            _logger.LogInformation("Tentativa de confirmação de email: {Email}", email);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Email e token são obrigatórios");
            }

            // 2. Buscar usuário por email
            var user = await _userRepository.ObterPorEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Email não encontrado para confirmação: {Email}", email);
                throw new InvalidOperationException("Token inválido ou expirado");
            }

            // 3. Verificar se já está confirmado
            if (user.EmailConfirmado)
            {
                _logger.LogInformation("Email já confirmado para usuário: {UserId}", user.Id);
                return; // Não é erro, apenas já está confirmado
            }

            // 4. Validar token de confirmação
            if (!_tokenService.ValidateEmailConfirmationToken(user, token))
            {
                _logger.LogWarning("Token de confirmação inválido para usuário: {UserId}", user.Id);
                
                // Registrar tentativa suspeita
                user.RegistrarLoginFalhado();
                _userRepository.Atualizar(user);
                await _userRepository.SalvarAlteracoesAsync();
                
                throw new InvalidOperationException("Token inválido ou expirado");
            }

            // 5. Confirmar email
            user.ConfirmarEmail();

            // 6. Ativar usuário se estava inativo (comum em novos cadastros)
            if (!user.Ativo)
            {
                user.Ativar();
                _logger.LogInformation("Usuário ativado durante confirmação de email: {UserId}", user.Id);
            }

            // 7. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Email confirmado com sucesso para usuário: {UserId}", user.Id);
        }
        catch (Exception ex) when (ex is not InvalidOperationException and not ArgumentException)
        {
            _logger.LogError(ex, "Erro inesperado durante confirmação de email: {Email}", email);
            throw new InvalidOperationException("Erro interno durante confirmação de email");
        }
    }
}

/// <summary>
/// Caso de uso para reenvio de confirmação de email.
/// Gera novo token e envia por email.
/// </summary>
public class ResendEmailConfirmationUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<ResendEmailConfirmationUseCase> _logger;

    public ResendEmailConfirmationUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<ResendEmailConfirmationUseCase> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de reenvio de confirmação de email
    /// </summary>
    public async Task ExecuteAsync(string email)
    {
        try
        {
            _logger.LogInformation("Reenvio de confirmação de email: {Email}", email);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Tentativa de reenvio com email vazio");
                return; // Não revelar erro por segurança
            }

            // 2. Buscar usuário por email
            var user = await _userRepository.ObterPorEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation("Email não encontrado para reenvio: {Email}", email);
                return; // Não revelar se o email existe
            }

            // 3. Verificar se já está confirmado
            if (user.EmailConfirmado)
            {
                _logger.LogInformation("Email já confirmado, não reenviando: {UserId}", user.Id);
                return; // Não é necessário reenviar
            }

            // 4. Verificar se o usuário está ativo
            if (!user.Ativo)
            {
                _logger.LogWarning("Tentativa de reenvio para usuário inativo: {UserId}", user.Id);
                return; // Não revelar status do usuário
            }

            // 5. Gerar novo token de confirmação
            var confirmationToken = _tokenService.GenerateEmailConfirmationToken(user);

            // 6. TODO: Enviar email com token de confirmação
            // Por enquanto, apenas log para desenvolvimento
            _logger.LogInformation("Token de confirmação gerado para usuário: {UserId}", user.Id);
            _logger.LogDebug("Token de confirmação: {Token}", confirmationToken);

            _logger.LogInformation("Confirmação de email reenviada para usuário: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante reenvio de confirmação de email para: {Email}", email);
            // Não propagar erro para não revelar informações sensíveis
        }
    }
}