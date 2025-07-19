using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Repositories;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para logout de usuário.
/// Invalida tokens e sessões ativas.
/// </summary>
public class LogoutUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<LogoutUseCase> _logger;

    public LogoutUseCase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        ILogger<LogoutUseCase> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de logout
    /// </summary>
    public async Task ExecuteAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Iniciando processo de logout para usuário: {UserId}", userId);

            // 1. Buscar usuário
            var user = await _userRepository.ObterPorIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de logout para usuário não encontrado: {UserId}", userId);
                return; // Logout silencioso para usuário não encontrado
            }

            // 2. Limpar refresh token
            user.LimparRefreshToken();

            // 3. Invalidar todas as sessões ativas
            await _sessionRepository.InvalidarTodasSessoesUsuarioAsync(userId);

            // 4. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();
            await _sessionRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout para usuário: {UserId}", userId);
            throw new InvalidOperationException("Erro interno durante logout");
        }
    }

    /// <summary>
    /// Executa logout de uma sessão específica
    /// </summary>
    public async Task ExecuteAsync(Guid userId, Guid sessionId)
    {
        try
        {
            _logger.LogInformation("Iniciando logout de sessão específica {SessionId} para usuário: {UserId}", 
                sessionId, userId);

            // 1. Buscar e invalidar sessão específica
            var session = await _sessionRepository.ObterPorIdAsync(sessionId);
            if (session != null && session.UsuarioId == userId)
            {
                session.Invalidar();
                _sessionRepository.Atualizar(session);
                await _sessionRepository.SalvarAlteracoesAsync();
            }

            // 2. Se foi a última sessão ativa, limpar refresh token
            var activeSessions = await _sessionRepository.ObterSessoesAtivasPorUsuarioAsync(userId);
            if (!activeSessions.Any())
            {
                var user = await _userRepository.ObterPorIdAsync(userId);
                if (user != null)
                {
                    user.LimparRefreshToken();
                    _userRepository.Atualizar(user);
                    await _userRepository.SalvarAlteracoesAsync();
                }
            }

            _logger.LogInformation("Logout de sessão específica realizado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout de sessão específica");
            throw new InvalidOperationException("Erro interno durante logout");
        }
    }
}