using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Professores.Dominio.Eventos;

public record TituloAcademicoAdicionadoEvento(
    Guid ProfessorId,
    TipoTitulo TipoTitulo,
    string Curso,
    string Instituicao) : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}