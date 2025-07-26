using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Academico.Dominio.Eventos;

public record DisciplinaCriadaEvento(Guid DisciplinaId, string Nome, string Codigo) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record DisciplinaAtualizadaEvento(Guid DisciplinaId, string Nome) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record PreRequisitoAdicionadoEvento(Guid DisciplinaId, Guid PreRequisitoId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record PreRequisitoRemovidoEvento(Guid DisciplinaId, Guid PreRequisitoId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record DisciplinaInativadaEvento(Guid DisciplinaId, string Nome) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record DisciplinaReativadaEvento(Guid DisciplinaId, string Nome) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}