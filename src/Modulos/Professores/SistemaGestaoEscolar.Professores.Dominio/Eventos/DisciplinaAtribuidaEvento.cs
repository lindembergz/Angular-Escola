using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record DisciplinaAtribuidaEvento(
    Guid ProfessorId,
    Guid DisciplinaId,
    int CargaHorariaSemanal) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}