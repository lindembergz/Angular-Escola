using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record CargaHorariaDisciplinaAtualizadaEvento(
    Guid ProfessorId,
    Guid DisciplinaId,
    int CargaHorariaAnterior,
    int NovaCargaHoraria) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}