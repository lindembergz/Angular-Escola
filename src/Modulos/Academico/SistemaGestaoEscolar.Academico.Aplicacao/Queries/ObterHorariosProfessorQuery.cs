using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterHorariosProfessorQuery : IRequest<IEnumerable<HorarioReadDto>>
{
    public Guid ProfessorId { get; set; }
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
}