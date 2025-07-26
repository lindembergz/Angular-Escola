using SistemaGestaoEscolar.Shared.Domain.Events;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Dominio.Eventos;

public record HorarioCriadoEvento(Guid HorarioId, Guid TurmaId, Guid DisciplinaId, 
                                 Guid ProfessorId, SlotTempo SlotTempo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record HorarioAtualizadoEvento(Guid HorarioId, string Campo, string? NovoValor) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record ProfessorAlteradoNoHorarioEvento(Guid HorarioId, Guid ProfessorAnterior, 
                                              Guid NovoProfessor) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record SlotTempoAlteradoEvento(Guid HorarioId, SlotTempo SlotAnterior, 
                                     SlotTempo NovoSlot) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record HorarioCanceladoEvento(Guid HorarioId, Guid TurmaId, Guid DisciplinaId, 
                                    SlotTempo SlotTempo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record HorarioReativadoEvento(Guid HorarioId, Guid TurmaId, Guid DisciplinaId, 
                                    SlotTempo SlotTempo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}