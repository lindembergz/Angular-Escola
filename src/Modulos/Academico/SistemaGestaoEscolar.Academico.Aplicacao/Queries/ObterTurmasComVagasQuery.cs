using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterTurmasComVagasQuery : IRequest<IEnumerable<TurmaReadDto>>
{
    public Guid UnidadeEscolarId { get; set; }
}