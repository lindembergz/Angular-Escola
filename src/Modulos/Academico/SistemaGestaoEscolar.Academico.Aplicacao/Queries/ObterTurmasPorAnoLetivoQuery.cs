using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterTurmasPorAnoLetivoQuery : IRequest<IEnumerable<TurmaReadDto>>
{
    public int AnoLetivo { get; set; }
    public Guid? UnidadeEscolarId { get; set; }
}