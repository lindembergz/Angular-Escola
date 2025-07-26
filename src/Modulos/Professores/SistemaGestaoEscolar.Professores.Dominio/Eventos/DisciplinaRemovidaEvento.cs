using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record DisciplinaRemovidaEvento(
    Guid ProfessorId,
    Guid DisciplinaId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}