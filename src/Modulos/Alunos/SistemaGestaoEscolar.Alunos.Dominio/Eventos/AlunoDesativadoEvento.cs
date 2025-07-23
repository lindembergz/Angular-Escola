using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoDesativadoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public string Nome { get; }
    public string Motivo { get; }
    public DateTime OcorridoEm { get; }

    public AlunoDesativadoEvento(Guid alunoId, string nome, string motivo)
    {
        AlunoId = alunoId;
        Nome = nome;
        Motivo = motivo;
        OcorridoEm = DateTime.UtcNow;
    }
}