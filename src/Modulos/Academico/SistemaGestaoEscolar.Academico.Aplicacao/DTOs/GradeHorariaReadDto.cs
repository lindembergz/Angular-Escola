
using System.Collections.Generic;

namespace SistemaGestaoEscolar.Academico.Aplicacao.DTOs
{
    public class GradeHorariaReadDto
    {
        public Guid TurmaId { get; set; }
        public string NomeTurma { get; set; }
        public Dictionary<DayOfWeek, List<HorarioReadDto>> Horarios { get; set; } = new();
    }
}
