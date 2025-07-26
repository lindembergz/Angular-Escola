using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record ProfessorAtivadoEvento(
    Guid ProfessorId,
    string Nome) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}