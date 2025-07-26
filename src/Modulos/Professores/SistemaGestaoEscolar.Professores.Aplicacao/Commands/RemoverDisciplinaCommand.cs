using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Commands;

public record RemoverDisciplinaCommand(
    Guid ProfessorId,
    Guid DisciplinaId) : IRequest<Unit>;