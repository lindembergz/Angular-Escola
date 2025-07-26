
using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands
{
    public class MatricularAlunoCommand : IRequest<bool>
    {
        public Guid AlunoId { get; set; }
        public Guid TurmaId { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
