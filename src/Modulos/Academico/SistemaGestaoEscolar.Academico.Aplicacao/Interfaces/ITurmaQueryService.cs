using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Interfaces;

public interface ITurmaQueryService
{
    Task<(IEnumerable<TurmaResumoReadDto> turmas, int totalItems)> ObterTurmasComFiltrosAsync(
        Guid? escolaId = null,
        int? anoLetivo = null,
        string? serie = null,
        string? turno = null,
        bool? ativa = null,
        int page = 1,
        int pageSize = 10);
        
    Task<TurmaReadDto?> ObterTurmaDetalheAsync(Guid turmaId);
}