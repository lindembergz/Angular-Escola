using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Escolas.Dominio.Repositorios;

public interface IRepositorioEscola : IRepository<Escola>
{
    Task<Escola?> ObterPorIdAsync(Guid id);
    Task<Escola?> ObterPorCnpjAsync(CNPJ cnpj);
    Task<IEnumerable<Escola>> ObterPorRedeAsync(Guid redeEscolarId);
    Task<IEnumerable<Escola>> ObterPorTipoAsync(TipoEscola tipo);
    Task<IEnumerable<Escola>> ObterAtivasAsync();
    Task<IEnumerable<Escola>> ObterInativasAsync();
    Task<IEnumerable<Escola>> PesquisarPorNomeAsync(string nome);
    Task<IEnumerable<Escola>> ObterPorCidadeAsync(string cidade);
    Task<IEnumerable<Escola>> ObterPorEstadoAsync(string estado);
    Task<bool> ExisteCnpjAsync(CNPJ cnpj);
    Task<bool> ExisteNomeAsync(NomeEscola nome, Guid? excluirId = null);
    Task<int> ContarEscolasAtivasAsync();
    Task<int> ContarEscolasPorTipoAsync(TipoEscola tipo);
    Task<int> ContarEscolasPorRedeAsync(Guid redeEscolarId);
    Task<int> ContarEscolasPorEstadoAsync(string estado);
    Task AdicionarAsync(Escola escola);
    Task AtualizarAsync(Escola escola);
    Task RemoverAsync(Escola escola);
    Task<IEnumerable<Escola>> ObterComUnidadesAsync();
    Task<Escola?> ObterComUnidadesPorIdAsync(Guid id);
    
    // Métodos para relatórios e estatísticas
    Task<Dictionary<string, int>> ObterEstatisticasPorTipoAsync();
    Task<Dictionary<string, int>> ObterEstatisticasPorEstadoAsync();
    Task<IEnumerable<Escola>> ObterEscolasComMaiorCapacidadeAsync(int limite = 10);
    Task<IEnumerable<Escola>> ObterEscolasComMenorOcupacaoAsync(int limite = 10);
    
    // Métodos para busca avançada
    Task<IEnumerable<Escola>> BuscarAvancadaAsync(
        string? nome = null,
        TipoEscola? tipo = null,
        string? cidade = null,
        string? estado = null,
        Guid? redeEscolarId = null,
        bool? ativa = null,
        int? capacidadeMinima = null,
        int? capacidadeMaxima = null);
        
    // Métodos para paginação
    Task<(IEnumerable<Escola> Escolas, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        TipoEscola? filtroTipo = null,
        bool? filtroAtiva = null);
}