using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries;

public class VerificarConflitosHorarioQuery : IRequest<IEnumerable<HorarioReadDto>>
{
    public Guid ProfessorId { get; set; }
    public string? Sala { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public int AnoLetivo { get; set; }
    public int Semestre { get; set; }
    public Guid? ExcluirHorarioId { get; set; }
}