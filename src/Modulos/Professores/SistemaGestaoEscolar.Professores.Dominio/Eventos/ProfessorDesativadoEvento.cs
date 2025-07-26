using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record ProfessorDesativadoEvento(
    Guid ProfessorId,
    string Nome,
    string Motivo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}