using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class PesquisarDisciplinasQuery : IRequest<IEnumerable<DisciplinaReadDto>>
{
    public string Nome { get; set; } = string.Empty;
    public Guid EscolaId { get; set; }
}