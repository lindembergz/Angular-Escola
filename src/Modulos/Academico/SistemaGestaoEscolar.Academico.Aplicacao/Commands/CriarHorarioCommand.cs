
using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands
{
    public class CriarHorarioCommand : IRequest<Guid>
    {
        public Guid TurmaId { get; set; }
        public Guid DisciplinaId { get; set; }
        public Guid ProfessorId { get; set; }
        public DayOfWeek DiaDaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFim { get; set; }
        public string? Sala { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
    }
}
