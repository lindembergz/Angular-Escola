using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record ProfessorTransferidoEvento(
    Guid ProfessorId,
    Guid EscolaAnteriorId,
    Guid NovaEscolaId,
    string Motivo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}