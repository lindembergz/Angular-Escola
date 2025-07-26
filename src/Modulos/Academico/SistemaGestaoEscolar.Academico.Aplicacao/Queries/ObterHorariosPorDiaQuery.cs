using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class ObterHorariosPorDiaQuery : IRequest<IEnumerable<HorarioReadDto>>
{
    public DayOfWeek DiaSemana { get; set; }
    public Guid? EscolaId { get; set; }
}