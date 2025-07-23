using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoMatriculadoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public Guid MatriculaId { get; }
    public Guid TurmaId { get; }
    public int AnoLetivo { get; }
    public DateTime OcorridoEm { get; }

    public AlunoMatriculadoEvento(Guid alunoId, Guid matriculaId, Guid turmaId, int anoLetivo)
    {
        AlunoId = alunoId;
        MatriculaId = matriculaId;
        TurmaId = turmaId;
        AnoLetivo = anoLetivo;
        OcorridoEm = DateTime.UtcNow;
    }
}