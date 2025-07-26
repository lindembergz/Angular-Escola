
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries
{
    public class ObterGradeHorariaQuery : IRequest<GradeHorariaReadDto>
    {
        public Guid TurmaId { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
    }
}
