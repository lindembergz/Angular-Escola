using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterDisciplinasObrigatoriasQuery : IRequest<IEnumerable<DisciplinaReadDto>>
{
    public Guid EscolaId { get; set; }
}