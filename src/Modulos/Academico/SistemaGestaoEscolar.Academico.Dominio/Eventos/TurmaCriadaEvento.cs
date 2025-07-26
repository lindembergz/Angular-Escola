using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Academico.Dominio.Eventos;

public record TurmaCriadaEvento(Guid TurmaId, string NomeTurma, string Serie) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record AlunoMatriculadoNaTurmaEvento(Guid TurmaId, Guid AlunoId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record AlunoRemovidoDaTurmaEvento(Guid TurmaId, Guid AlunoId) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record TurmaInativadaEvento(Guid TurmaId, string NomeTurma) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}

public record TurmaReativadaEvento(Guid TurmaId, string NomeTurma) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}