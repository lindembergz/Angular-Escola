using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Domain.Repositories;

/// <summary>
/// Interface do repositório para operações com usuários.
/// Define contratos para persistência seguindo DDD patterns.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    Task<User?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por email
    /// </summary>
    Task<User?> ObterPorEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário por email (string)
    /// </summary>
    Task<User?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe um usuário com o email especificado
    /// </summary>
    Task<bool> ExistePorEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe um usuário com o email especificado (string)
    /// </summary>
    Task<bool> ExistePorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários por papel
    /// </summary>
    Task<IEnumerable<User>> ObterPorPerfilAsync(UserRole perfil, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários por escola
    /// </summary>
    Task<IEnumerable<User>> ObterPorEscolaAsync(Guid escolaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários ativos
    /// </summary>
    Task<IEnumerable<User>> ObterUsuariosAtivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários inativos há mais de X dias
    /// </summary>
    Task<IEnumerable<User>> ObterUsuariosInativosAsync(int diasSemLogin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários com contas bloqueadas
    /// </summary>
    Task<IEnumerable<User>> ObterUsuariosBloqueadosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários que precisam alterar a senha
    /// </summary>
    Task<IEnumerable<User>> ObterUsuariosQuePrecisamAlterarSenhaAsync(int maxDiasSemMudanca, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca usuários por termo (nome, email)
    /// </summary>
    Task<IEnumerable<User>> BuscarAsync(string termoBusca, int pular = 0, int pegar = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários paginados
    /// </summary>
    Task<(IEnumerable<User> Usuarios, int TotalRegistros)> ObterPaginadoAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários paginados por escola
    /// </summary>
    Task<(IEnumerable<User> Usuarios, int TotalRegistros)> ObterPaginadoPorEscolaAsync(Guid escolaId, int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona um novo usuário
    /// </summary>
    Task AdicionarAsync(User usuario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    void Atualizar(User usuario);

    /// <summary>
    /// Remove um usuário (soft delete)
    /// </summary>
    void Remover(User usuario);

    /// <summary>
    /// Remove um usuário por ID (soft delete)
    /// </summary>
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva as alterações no contexto
    /// </summary>
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém estatísticas de usuários
    /// </summary>
    Task<EstatisticasUsuario> ObterEstatisticasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuários com refresh token válido
    /// </summary>
    Task<IEnumerable<User>> ObterUsuariosComRefreshTokenValidoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Limpa refresh tokens expirados
    /// </summary>
    Task LimparRefreshTokensExpiradosAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Estatísticas de usuários do sistema
/// </summary>
public record EstatisticasUsuario(
    int TotalUsuarios,
    int UsuariosAtivos,
    int UsuariosInativos,
    int UsuariosBloqueados,
    int UsuariosQuePrecisamAlterarSenha,
    Dictionary<string, int> UsuariosPorPerfil,
    Dictionary<Guid, int> UsuariosPorEscola
);