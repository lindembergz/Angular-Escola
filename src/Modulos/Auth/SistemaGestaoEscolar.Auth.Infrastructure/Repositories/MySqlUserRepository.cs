using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;
using SistemaGestaoEscolar.Auth.Infrastructure.Context;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de usuários para MySQL.
/// Segue padrões de Repository Pattern e DDD.
/// </summary>
public class MySqlUserRepository : IUserRepository
{
    private readonly AuthDbContext _context;
    private readonly ILogger<MySqlUserRepository> _logger;

    public MySqlUserRepository(AuthDbContext context, ILogger<MySqlUserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    public async Task<User?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtém um usuário por email
    /// </summary>
    public async Task<User?> ObterPorEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await ObterPorEmailAsync(email.Value, cancellationToken);
    }

    /// <summary>
    /// Obtém um usuário por email (string)
    /// </summary>
    public async Task<User?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            
            return await _context.Users
                .Include(u => u.Sessions)
                .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Verifica se existe um usuário com o email especificado
    /// </summary>
    public async Task<bool> ExistePorEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await ExistePorEmailAsync(email.Value, cancellationToken);
    }

    /// <summary>
    /// Verifica se existe um usuário com o email especificado (string)
    /// </summary>
    public async Task<bool> ExistePorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            
            return await _context.Users
                .AnyAsync(u => u.Email.Value == normalizedEmail, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência de usuário por email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários por papel
    /// </summary>
    public async Task<IEnumerable<User>> ObterPorPerfilAsync(UserRole perfil, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .Where(u => u.Perfil.Code == perfil.Code)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários por papel: {Role}", perfil.Code);
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários por escola
    /// </summary>
    public async Task<IEnumerable<User>> ObterPorEscolaAsync(Guid escolaId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .Where(u => u.EscolaId == escolaId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários por escola: {SchoolId}", escolaId);
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários ativos
    /// </summary>
    public async Task<IEnumerable<User>> ObterUsuariosAtivosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .Where(u => u.Ativo)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários ativos");
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários inativos há mais de X dias
    /// </summary>
    public async Task<IEnumerable<User>> ObterUsuariosInativosAsync(int diasSemLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-diasSemLogin);
            
            return await _context.Users
                .Where(u => u.Ativo && 
                           (u.UltimoLoginEm == null || u.UltimoLoginEm < cutoffDate))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários inativos");
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários com contas bloqueadas
    /// </summary>
    public async Task<IEnumerable<User>> ObterUsuariosBloqueadosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            
            return await _context.Users
                .Where(u => u.BloqueadoAte != null && u.BloqueadoAte > now)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários bloqueados");
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários que precisam alterar a senha
    /// </summary>
    public async Task<IEnumerable<User>> ObterUsuariosQuePrecisamAlterarSenhaAsync(int maxDiasSemMudanca, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-maxDiasSemMudanca);
            
            return await _context.Users
                .Where(u => u.Ativo && 
                           (u.UltimaMudancaSenhaEm == null || u.UltimaMudancaSenhaEm < cutoffDate))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários que precisam alterar senha");
            throw;
        }
    }

    /// <summary>
    /// Busca usuários por termo (nome, email)
    /// </summary>
    public async Task<IEnumerable<User>> BuscarAsync(string termoBusca, int pular = 0, int pegar = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(termoBusca))
                return Array.Empty<User>();

            var normalizedTerm = termoBusca.Trim().ToLowerInvariant();
            
            return await _context.Users
                .Where(u => u.PrimeiroNome.ToLower().Contains(normalizedTerm) ||
                           u.UltimoNome.ToLower().Contains(normalizedTerm) ||
                           u.Email.Value.Contains(normalizedTerm))
                .OrderBy(u => u.PrimeiroNome)
                .ThenBy(u => u.UltimoNome)
                .Skip(pular)
                .Take(Math.Min(pegar, 100)) // Máximo 100 resultados
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários por termo: {SearchTerm}", termoBusca);
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários paginados
    /// </summary>
    public async Task<(IEnumerable<User> Usuarios, int TotalRegistros)> ObterPaginadoAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.AsQueryable();
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var users = await query
                .OrderBy(u => u.PrimeiroNome)
                .ThenBy(u => u.UltimoNome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync(cancellationToken);
            
            return (users, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários paginados");
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários paginados por escola
    /// </summary>
    public async Task<(IEnumerable<User> Usuarios, int TotalRegistros)> ObterPaginadoPorEscolaAsync(Guid escolaId, int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.Where(u => u.EscolaId == escolaId);
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var users = await query
                .OrderBy(u => u.PrimeiroNome)
                .ThenBy(u => u.UltimoNome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync(cancellationToken);
            
            return (users, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários paginados por escola: {SchoolId}", escolaId);
            throw;
        }
    }

    /// <summary>
    /// Adiciona um novo usuário
    /// </summary>
    public async Task AdicionarAsync(User usuario, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Users.AddAsync(usuario, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar usuário: {UserId}", usuario.Id);
            throw;
        }
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    public void Atualizar(User usuario)
    {
        try
        {
            _context.Users.Update(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: {UserId}", usuario.Id);
            throw;
        }
    }

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    public void Remover(User usuario)
    {
        try
        {
            usuario.Desativar();
            _context.Users.Update(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover usuário: {UserId}", usuario.Id);
            throw;
        }
    }

    /// <summary>
    /// Remove um usuário por ID (soft delete)
    /// </summary>
    public async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await ObterPorIdAsync(id, cancellationToken);
            if (user != null)
            {
                Remover(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover usuário por ID: {UserId}", id);
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
            _logger.LogError(ex, "Erro ao salvar alterações no contexto de usuários");
            throw;
        }
    }

    /// <summary>
    /// Obtém estatísticas de usuários
    /// </summary>
    public async Task<EstatisticasUsuario> ObterEstatisticasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalUsers = await _context.Users.CountAsync(cancellationToken);
            var activeUsers = await _context.Users.CountAsync(u => u.Ativo, cancellationToken);
            var inactiveUsers = totalUsers - activeUsers;
            
            var now = DateTime.UtcNow;
            var lockedUsers = await _context.Users
                .CountAsync(u => u.BloqueadoAte != null && u.BloqueadoAte > now, cancellationToken);
            
            var cutoffDate = DateTime.UtcNow.AddDays(-90);
            var usersNeedingPasswordChange = await _context.Users
                .CountAsync(u => u.Ativo && 
                               (u.UltimaMudancaSenhaEm == null || u.UltimaMudancaSenhaEm < cutoffDate), 
                           cancellationToken);
            
            var usersByRole = await _context.Users
                .GroupBy(u => u.Perfil.Code)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Role, x => x.Count, cancellationToken);
            
            var usersBySchool = await _context.Users
                .Where(u => u.EscolaId != null)
                .GroupBy(u => u.EscolaId!.Value)
                .Select(g => new { SchoolId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SchoolId, x => x.Count, cancellationToken);
            
            return new EstatisticasUsuario(
                totalUsers,
                activeUsers,
                inactiveUsers,
                lockedUsers,
                usersNeedingPasswordChange,
                usersByRole,
                usersBySchool
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas de usuários");
            throw;
        }
    }

    /// <summary>
    /// Obtém usuários com refresh token válido
    /// </summary>
    public async Task<IEnumerable<User>> ObterUsuariosComRefreshTokenValidoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            
            return await _context.Users
                .Where(u => u.RefreshToken != null && 
                           u.RefreshTokenExpiraEm != null && 
                           u.RefreshTokenExpiraEm > now)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com refresh token válido");
            throw;
        }
    }

    /// <summary>
    /// Limpa refresh tokens expirados
    /// </summary>
    public async Task LimparRefreshTokensExpiradosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            
            var usersWithExpiredTokens = await _context.Users
                .Where(u => u.RefreshToken != null && 
                           (u.RefreshTokenExpiraEm == null || u.RefreshTokenExpiraEm <= now))
                .ToListAsync(cancellationToken);
            
            foreach (var user in usersWithExpiredTokens)
            {
                user.LimparRefreshToken();
            }
            
            if (usersWithExpiredTokens.Any())
            {
                await SalvarAlteracoesAsync(cancellationToken);
                _logger.LogInformation("Limpeza de {Count} refresh tokens expirados", usersWithExpiredTokens.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao limpar refresh tokens expirados");
            throw;
        }
    }
}