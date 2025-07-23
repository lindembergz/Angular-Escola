using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class AlunoTransferidoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public Guid EscolaAnteriorId { get; }
    public Guid NovaEscolaId { get; }
    public string Motivo { get; }
    public DateTime OcorridoEm { get; }

    public AlunoTransferidoEvento(Guid alunoId, Guid escolaAnteriorId, Guid novaEscolaId, string motivo)
    {
        AlunoId = alunoId;
        EscolaAnteriorId = escolaAnteriorId;
        NovaEscolaId = novaEscolaId;
        Motivo = motivo;
        OcorridoEm = DateTime.UtcNow;
    }
}