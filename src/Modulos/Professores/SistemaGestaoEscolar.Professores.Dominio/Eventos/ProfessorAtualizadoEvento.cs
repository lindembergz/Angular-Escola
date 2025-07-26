using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record ProfessorAtualizadoEvento(
    Guid ProfessorId,
    string Campo,
    string ValorAnterior,
    string NovoValor) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}