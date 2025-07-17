using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Escolas.Dominio.Repositorios;

public interface IRepositorioRedeEscolar : IRepository<RedeEscolar>
{
    Task<RedeEscolar?> ObterPorIdAsync(Guid id);
    Task<RedeEscolar?> ObterPorCnpjAsync(CNPJ cnpjMantenedora);
    Task<IEnumerable<RedeEscolar>> ObterTodasAsync();
    Task<IEnumerable<RedeEscolar>> ObterAtivasAsync();
    Task<IEnumerable<RedeEscolar>> ObterInativasAsync();
    Task<IEnumerable<RedeEscolar>> PesquisarPorNomeAsync(string nome);
    Task<IEnumerable<RedeEscolar>> ObterPorCidadeSedeAsync(string cidade);
    Task<IEnumerable<RedeEscolar>> ObterPorEstadoSedeAsync(string estado);
    Task<bool> ExisteCnpjAsync(CNPJ cnpjMantenedora);
    Task<bool> ExisteNomeAsync(NomeEscola nome, Guid? excluirId = null);
    Task<int> ContarRedesAtivasAsync();
    Task<int> ContarTotalEscolasAsync();
    Task AdicionarAsync(RedeEscolar redeEscolar);
    Task AtualizarAsync(RedeEscolar redeEscolar);
    Task RemoverAsync(RedeEscolar redeEscolar);
    Task<RedeEscolar?> ObterComEscolasPorIdAsync(Guid id);
    Task<IEnumerable<RedeEscolar>> ObterComEscolasAsync();
    
    // Métodos para estatísticas e relatórios
    Task<Dictionary<string, int>> ObterEstatisticasEscolasPorRedeAsync();
    Task<Dictionary<string, int>> ObterEstatisticasPorEstadoSedeAsync();
    Task<IEnumerable<RedeEscolar>> ObterRedesComMaisEscolasAsync(int limite = 10);
    Task<IEnumerable<RedeEscolar>> ObterRedesComMenosEscolasAsync(int limite = 10);
    Task<int> ObterTotalAlunosRedeAsync(Guid redeId);
    Task<int> ObterTotalProfessoresRedeAsync(Guid redeId);
    
    // Métodos para busca avançada
    Task<IEnumerable<RedeEscolar>> BuscarAvancadaAsync(
        string? nome = null,
        string? cidadeSede = null,
        string? estadoSede = null,
        bool? ativa = null,
        int? minimoEscolas = null,
        int? maximoEscolas = null);
        
    // Métodos para paginação
    Task<(IEnumerable<RedeEscolar> Redes, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        string? filtroCidade = null,
        bool? filtroAtiva = null);
        
    // Métodos específicos para gestão de redes
    Task<bool> PodeAdicionarEscolaAsync(Guid redeId, CNPJ cnpjEscola);
    Task<bool> PodeRemoverEscolaAsync(Guid redeId, Guid escolaId);
    Task<IEnumerable<Escola>> ObterEscolasDisponiveis(); // Escolas sem rede
    Task<RedeEscolar?> ObterRedeDaEscolaAsync(Guid escolaId);
    
    // Métodos para consolidação de dados
    Task<Dictionary<TipoEscola, int>> ObterDistribuicaoTiposEscolasAsync(Guid redeId);
    Task<Dictionary<string, int>> ObterDistribuicaoGeograficaAsync(Guid redeId);
    Task<int> ObterCapacidadeTotalAsync(Guid redeId);
    Task<int> ObterOcupacaoTotalAsync(Guid redeId);
    Task<double> ObterPercentualOcupacaoRedeAsync(Guid redeId);
}