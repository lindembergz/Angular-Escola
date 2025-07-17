using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.Servicos;

public interface IServicoAplicacaoEscola
{
    // Operações de Escola
    Task<EscolaRespostaDto> CriarEscolaAsync(CriarEscolaDto dto);
    Task<EscolaRespostaDto?> ObterEscolaPorIdAsync(Guid id);
    Task<IEnumerable<EscolaRespostaDto>> ObterTodasEscolasAsync();
    Task<IEnumerable<EscolaRespostaDto>> ObterEscolasAtivasAsync();
    Task<IEnumerable<EscolaRespostaDto>> ObterEscolasPorTipoAsync(string tipo);
    Task<IEnumerable<EscolaRespostaDto>> PesquisarEscolasPorNomeAsync(string nome);
    Task<EscolaRespostaDto> AtualizarEscolaAsync(Guid id, CriarEscolaDto dto);
    Task DesativarEscolaAsync(Guid id);
    Task AtivarEscolaAsync(Guid id);
    Task RemoverEscolaAsync(Guid id);

    // Operações de Rede Escolar
    Task<RedeEscolarRespostaDto> CriarRedeEscolarAsync(CriarRedeEscolarDto dto);
    Task<RedeEscolarRespostaDto?> ObterRedeEscolarPorIdAsync(Guid id);
    Task<IEnumerable<RedeEscolarRespostaDto>> ObterTodasRedesEscolaresAsync();
    Task<IEnumerable<RedeEscolarRespostaDto>> ObterRedesEscolaresAtivasAsync();
    Task<RedeEscolarRespostaDto> AtualizarRedeEscolarAsync(Guid id, CriarRedeEscolarDto dto);
    Task DesativarRedeEscolarAsync(Guid id);
    Task AtivarRedeEscolarAsync(Guid id);
    Task RemoverRedeEscolarAsync(Guid id);

    // Operações de Unidade Escolar
    Task<UnidadeEscolarRespostaDto> AdicionarUnidadeEscolarAsync(AdicionarUnidadeEscolarDto dto);
    Task RemoverUnidadeEscolarAsync(Guid escolaId, Guid unidadeId);

    // Operações de Relacionamento
    Task<IEnumerable<EscolaRespostaDto>> ObterEscolasPorRedeAsync(Guid redeEscolarId);
    Task AssociarEscolaRedeAsync(Guid escolaId, Guid redeEscolarId);
    Task DesassociarEscolaRedeAsync(Guid escolaId);

    // Operações de Consulta Avançada
    Task<(IEnumerable<EscolaRespostaDto> Escolas, int Total)> ObterEscolasPaginadoAsync(
        int pagina, int tamanhoPagina, string? filtroNome = null, string? filtroTipo = null, bool? filtroAtiva = null);
    
    Task<(IEnumerable<RedeEscolarRespostaDto> Redes, int Total)> ObterRedesEscolaresPaginadoAsync(
        int pagina, int tamanhoPagina, string? filtroNome = null, bool? filtroAtiva = null);

    // Estatísticas
    Task<Dictionary<string, int>> ObterEstatisticasEscolasPorTipoAsync();
    Task<Dictionary<string, int>> ObterEstatisticasEscolasPorEstadoAsync();
    Task<int> ObterTotalEscolasAtivasAsync();
    Task<int> ObterTotalRedesEscolaresAtivasAsync();
}