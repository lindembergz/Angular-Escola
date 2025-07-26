using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterDisciplinasPorSerieQuery : IRequest<IEnumerable<DisciplinaReadDto>>
{
    public int TipoSerie { get; set; }
    public int AnoSerie { get; set; }
    public Guid EscolaId { get; set; }
}