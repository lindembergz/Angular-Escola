using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record ProfessorCriadoEvento(
    Guid ProfessorId,
    string Nome,
    string Cpf,
    Guid EscolaId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}