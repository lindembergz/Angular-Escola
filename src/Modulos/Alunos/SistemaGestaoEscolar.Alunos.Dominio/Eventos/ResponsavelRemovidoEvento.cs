using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Dominio.Eventos;

public class ResponsavelRemovidoEvento : IDomainEvent
{
    public Guid AlunoId { get; }
    public Guid ResponsavelId { get; }
    public string NomeResponsavel { get; }
    public DateTime OcorridoEm { get; }

    public ResponsavelRemovidoEvento(Guid alunoId, Guid responsavelId, string nomeResponsavel)
    {
        AlunoId = alunoId;
        ResponsavelId = responsavelId;
        NomeResponsavel = nomeResponsavel;
        OcorridoEm = DateTime.UtcNow;
    }
}