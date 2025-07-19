using SistemaGestaoEscolar.Auth.Domain.Entities;

namespace SistemaGestaoEscolar.Auth.Domain.Repositories;

/// <summary>
/// Interface do repositório para operações com sessões de usuário.
/// Define contratos para persistência de sessões seguindo DDD patterns.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Obtém uma sessão por ID
    /// </summary>
    Task<Session?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões ativas de um usuário
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as sessões de um usuário
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões por endereço IP
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesPorEnderecoIpAsync(string enderecoIp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões expiradas
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesExpiradasAsync(int maxMinutosInativo = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões suspeitas
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesSuspeitasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões de longa duração
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesLongaDuracaoAsync(int maxHoras = 8, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões ativas no sistema
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesAtivasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém sessões por período
    /// </summary>
    Task<IEnumerable<Session>> ObterSessoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta sessões ativas por usuário
    /// </summary>
    Task<int> ContarSessoesAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta total de sessões ativas
    /// </summary>
    Task<int> ContarSessoesAtivasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se existe sessão ativa para o usuário no dispositivo
    /// </summary>
    Task<bool> TemSessaoAtivaAsync(Guid usuarioId, string enderecoIp, string agenteUsuario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a sessão mais recente de um usuário
    /// </summary>
    Task<Session?> ObterUltimaSessaoPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova sessão
    /// </summary>
    Task AdicionarAsync(Session sessao, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza uma sessão existente
    /// </summary>
    void Atualizar(Session sessao);

    /// <summary>
    /// Remove uma sessão
    /// </summary>
    void Remover(Session sessao);

    /// <summary>
    /// Remove sessões por ID
    /// </summary>
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove todas as sessões de um usuário
    /// </summary>
    Task RemoverTodasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove sessões expiradas
    /// </summary>
    Task RemoverSessoesExpiradasAsync(int maxMinutosInativo = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove sessões antigas
    /// </summary>
    Task RemoverSessoesAntigasAsync(int maxDiasAntigo = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalida todas as sessões ativas de um usuário
    /// </summary>
    Task InvalidarTodasSessoesUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalida sessões específicas
    /// </summary>
    Task InvalidarSessoesAsync(IEnumerable<Guid> sessoesIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva as alterações no contexto
    /// </summary>
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém estatísticas de sessões
    /// </summary>
    Task<EstatisticasSessao> ObterEstatisticasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém relatório de atividade de sessões
    /// </summary>
    Task<IEnumerable<AtividadeSessao>> ObterRelatorioAtividadeAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
}

/// <summary>
/// Estatísticas de sessões do sistema
/// </summary>
public record EstatisticasSessao(
    int TotalSessoes,
    int SessoesAtivas,
    int SessoesExpiradas,
    int SessoesSuspeitas,
    int SessoesLongaDuracao,
    Dictionary<string, int> SessoesPorDispositivo,
    Dictionary<string, int> SessoesPorLocalizacao,
    TimeSpan DuracaoMediaSessao,
    int UsuariosUnicos,
    int PicoUsuariosConcorrentes
);

/// <summary>
/// Atividade de sessão para relatórios
/// </summary>
public record AtividadeSessao(
    Guid SessaoId,
    Guid UsuarioId,
    string EmailUsuario,
    string EnderecoIp,
    string InfoDispositivo,
    DateTime IniciadaEm,
    DateTime? FinalizadaEm,
    TimeSpan Duracao,
    bool Ativa,
    bool Suspeita
);