using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands.Turma;

public record InativarTurmaCommand(Guid Id) : IRequest<Unit>;