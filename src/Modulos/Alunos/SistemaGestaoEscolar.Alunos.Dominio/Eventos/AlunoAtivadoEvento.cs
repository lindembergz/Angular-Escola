using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoAtivadoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public string Nome { get; }
    public DateTime OcorridoEm { get; }

    public AlunoAtivadoEvento(Guid alunoId, string nome)
    {
        AlunoId = alunoId;
        Nome = nome;
        OcorridoEm = DateTime.UtcNow;
    }
}