using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Infrastructure.Context;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de sessões para MySQL.
/// Segue padrões de Repository Pattern e DDD.
/// </summary>
public class MySqlSessionRepository : ISessionRepository
{
    private readonly AuthDbContext _context;
    private readonly ILogger<MySqlSessionRepository> _logger;

    public MySqlSessionRepository(AuthDbContext context, ILogger<MySqlSessionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém uma sessão por ID
    /// </summary>
    public async Task<Session?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessão por ID: {SessionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões ativas de um usuário
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.UsuarioId == usuarioId && s.Ativa)
                .OrderByDescending(s => s.UltimaAtividadeEm)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões ativas do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Obtém todas as sessões de um usuário
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.IniciadaEm)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões por endereço IP
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesPorEnderecoIpAsync(string enderecoIp, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.EnderecoIp == enderecoIp)
                .OrderByDescending(s => s.IniciadaEm)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões por IP: {IpAddress}", enderecoIp);
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões expiradas
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesExpiradasAsync(int maxMinutosInativo = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-maxMinutosInativo);
            
            return await _context.Sessions
                .Where(s => s.Ativa && s.UltimaAtividadeEm < cutoffTime)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões expiradas");
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões suspeitas
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesSuspeitasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            
            // Sessões suspeitas: muito longas sem atividade ou extremamente longas
            return await _context.Sessions
                .Where(s => (s.Ativa && s.UltimaAtividadeEm < oneDayAgo) ||
                           s.IniciadaEm < oneWeekAgo ||
                           s.AgenteUsuario.ToLower().Contains("bot") ||
                           s.AgenteUsuario.ToLower().Contains("crawler") ||
                           s.AgenteUsuario.ToLower().Contains("spider"))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões suspeitas");
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões de longa duração
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesLongaDuracaoAsync(int maxHoras = 8, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-maxHoras);
            
            return await _context.Sessions
                .Where(s => s.Ativa && s.IniciadaEm < cutoffTime)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões de longa duração");
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões ativas no sistema
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesAtivasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.Ativa)
                .OrderByDescending(s => s.UltimaAtividadeEm)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões ativas");
            throw;
        }
    }

    /// <summary>
    /// Obtém sessões por período
    /// </summary>
    public async Task<IEnumerable<Session>> ObterSessoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.IniciadaEm >= dataInicio && s.IniciadaEm <= dataFim)
                .OrderBy(s => s.IniciadaEm)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessões por período");
            throw;
        }
    }

    /// <summary>
    /// Conta sessões ativas por usuário
    /// </summary>
    public async Task<int> ContarSessoesAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .CountAsync(s => s.UsuarioId == usuarioId && s.Ativa, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar sessões ativas do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Conta total de sessões ativas
    /// </summary>
    public async Task<int> ContarSessoesAtivasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .CountAsync(s => s.Ativa, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao contar sessões ativas");
            throw;
        }
    }

    /// <summary>
    /// Verifica se existe sessão ativa para o usuário no dispositivo
    /// </summary>
    public async Task<bool> TemSessaoAtivaAsync(Guid usuarioId, string enderecoIp, string agenteUsuario, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .AnyAsync(s => s.UsuarioId == usuarioId && 
                              s.Ativa && 
                              s.EnderecoIp == enderecoIp && 
                              s.AgenteUsuario == agenteUsuario, 
                         cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar sessão ativa para usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Obtém a sessão mais recente de um usuário
    /// </summary>
    public async Task<Session?> ObterUltimaSessaoPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Sessions
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.IniciadaEm)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar sessão mais recente do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Adiciona uma nova sessão
    /// </summary>
    public async Task AdicionarAsync(Session sessao, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Sessions.AddAsync(sessao, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar sessão: {SessionId}", sessao.Id);
            throw;
        }
    }

    /// <summary>
    /// Atualiza uma sessão existente
    /// </summary>
    public void Atualizar(Session sessao)
    {
        try
        {
            _context.Sessions.Update(sessao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar sessão: {SessionId}", sessao.Id);
            throw;
        }
    }

    /// <summary>
    /// Remove uma sessão
    /// </summary>
    public void Remover(Session sessao)
    {
        try
        {
            _context.Sessions.Remove(sessao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover sessão: {SessionId}", sessao.Id);
            throw;
        }
    }

    /// <summary>
    /// Remove sessões por ID
    /// </summary>
    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var session = await ObterPorIdAsync(id, cancellationToken);
            if (session != null)
            {
                Remover(session);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover sessão por ID: {SessionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Remove todas as sessões de um usuário
    /// </summary>
    public async Task RemoverTodasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _context.Sessions
                .Where(s => s.UsuarioId == usuarioId)
                .ToListAsync(cancellationToken);
                
            _context.Sessions.RemoveRange(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover todas as sessões do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Remove sessões expiradas
    /// </summary>
    public async Task RemoverSessoesExpiradasAsync(int maxMinutosInativo = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var expiredSessions = await ObterSessoesExpiradasAsync(maxMinutosInativo, cancellationToken);
            
            if (expiredSessions.Any())
            {
                _context.Sessions.RemoveRange(expiredSessions);
                _logger.LogInformation("Removendo {Count} sessões expiradas", expiredSessions.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover sessões expiradas");
            throw;
        }
    }

    /// <summary>
    /// Remove sessões antigas
    /// </summary>
    public async Task RemoverSessoesAntigasAsync(int maxDiasAntigo = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-maxDiasAntigo);
            
            var oldSessions = await _context.Sessions
                .Where(s => s.IniciadaEm < cutoffDate)
                .ToListAsync(cancellationToken);
                
            if (oldSessions.Any())
            {
                _context.Sessions.RemoveRange(oldSessions);
                _logger.LogInformation("Removendo {Count} sessões antigas", oldSessions.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover sessões antigas");
            throw;
        }
    }

    /// <summary>
    /// Invalida todas as sessões ativas de um usuário
    /// </summary>
    public async Task InvalidarTodasSessoesUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var activeSessions = await _context.Sessions
                .Where(s => s.UsuarioId == usuarioId && s.Ativa)
                .ToListAsync(cancellationToken);
                
            foreach (var session in activeSessions)
            {
                session.Invalidar();
            }
            
            if (activeSessions.Any())
            {
                _logger.LogInformation("Invalidando {Count} sessões ativas do usuário: {UserId}", 
                    activeSessions.Count, usuarioId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao invalidar sessões do usuário: {UserId}", usuarioId);
            throw;
        }
    }

    /// <summary>
    /// Invalida sessões específicas
    /// </summary>
    public async Task InvalidarSessoesAsync(IEnumerable<Guid> sessoesIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _context.Sessions
                .Where(s => sessoesIds.Contains(s.Id) && s.Ativa)
                .ToListAsync(cancellationToken);
                
            foreach (var session in sessions)
            {
                session.Invalidar();
            }
            
            if (sessions.Any())
            {
                _logger.LogInformation("Invalidando {Count} sessões específicas", sessions.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao invalidar sessões específicas");
            throw;
        }
    }

    /// <summary>
    /// Salva as alterações no contexto
    /// </summary>
    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no contexto de sessões");
            throw;
        }
    }

    /// <summary>
    /// Obtém estatísticas de sessões
    /// </summary>
    public async Task<EstatisticasSessao> ObterEstatisticasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalSessions = await _context.Sessions.CountAsync(cancellationToken);
            var activeSessions = await _context.Sessions.CountAsync(s => s.Ativa, cancellationToken);
            var expiredSessions = await ObterSessoesExpiradasAsync(30, cancellationToken);
            var suspiciousSessions = await ObterSessoesSuspeitasAsync(cancellationToken);
            var longRunningSessions = await ObterSessoesLongaDuracaoAsync(8, cancellationToken);
            
            var sessionsByDevice = await _context.Sessions
                .Where(s => s.InfoDispositivo != null)
                .GroupBy(s => s.InfoDispositivo!)
                .Select(g => new { Device = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Device, x => x.Count, cancellationToken);
                
            var sessionsByLocation = await _context.Sessions
                .Where(s => s.Localizacao != null)
                .GroupBy(s => s.Localizacao!)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Location, x => x.Count, cancellationToken);
            
            // Calcular duração média das sessões encerradas
            var completedSessions = await _context.Sessions
                .Where(s => s.FinalizadaEm != null)
                .Select(s => EF.Functions.DateDiffSecond(s.IniciadaEm, s.FinalizadaEm!.Value))
                .ToListAsync(cancellationToken);
                
            var averageDuration = completedSessions.Any() 
                ? TimeSpan.FromSeconds(completedSessions.Average())
                : TimeSpan.Zero;
            
            var uniqueUsers = await _context.Sessions
                .Select(s => s.UsuarioId)
                .Distinct()
                .CountAsync(cancellationToken);
            
            // Peak de usuários simultâneos (aproximação baseada em sessões ativas)
            var concurrentPeakUsers = activeSessions; // Simplificação
            
            return new EstatisticasSessao(
                totalSessions,
                activeSessions,
                expiredSessions.Count(),
                suspiciousSessions.Count(),
                longRunningSessions.Count(),
                sessionsByDevice,
                sessionsByLocation,
                averageDuration,
                uniqueUsers,
                concurrentPeakUsers
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas de sessões");
            throw;
        }
    }

    /// <summary>
    /// Obtém relatório de atividade de sessões
    /// </summary>
    public async Task<IEnumerable<AtividadeSessao>> ObterRelatorioAtividadeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var sessions = await _context.Sessions
                .Where(s => s.IniciadaEm >= startDate && s.IniciadaEm <= endDate)
                .Join(_context.Users,
                      session => session.UsuarioId,
                      user => user.Id,
                      (session, user) => new AtividadeSessao(
                          session.Id,
                          session.UsuarioId,
                          user.Email.Value,
                          session.EnderecoIp,
                          session.InfoDispositivo ?? "Desconhecido",
                          session.IniciadaEm,
                          session.FinalizadaEm,
                          session.FinalizadaEm != null 
                              ? session.FinalizadaEm.Value - session.IniciadaEm
                              : DateTime.UtcNow - session.IniciadaEm,
                          session.Ativa,
                          session.AgenteUsuario.ToLower().Contains("bot") ||
                          session.AgenteUsuario.ToLower().Contains("crawler") ||
                          (session.Ativa && session.UltimaAtividadeEm < DateTime.UtcNow.AddDays(-1))
                      ))
                .OrderBy(sa => sa.IniciadaEm)
                .ToListAsync(cancellationToken);
                
            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter relatório de atividade de sessões");
            throw;
        }
    }
}