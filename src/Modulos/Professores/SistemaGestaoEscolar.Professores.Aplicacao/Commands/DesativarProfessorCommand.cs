using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Commands;

public record DesativarProfessorCommand(
    Guid Id,
    string Motivo) : IRequest<Unit>;