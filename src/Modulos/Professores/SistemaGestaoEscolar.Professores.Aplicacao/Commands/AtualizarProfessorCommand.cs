using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Commands;

public record AtualizarProfessorCommand(
    Guid Id,
    string Nome,
    string? Email = null,
    string? Telefone = null,
    string? Observacoes = null) : IRequest<Unit>;