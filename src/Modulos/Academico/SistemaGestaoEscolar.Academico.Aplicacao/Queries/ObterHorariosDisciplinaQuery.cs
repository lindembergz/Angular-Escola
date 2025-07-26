using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterHorariosDisciplinaQuery : IRequest<IEnumerable<HorarioReadDto>>
{
    public Guid DisciplinaId { get; set; }
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
}