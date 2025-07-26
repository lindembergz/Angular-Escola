using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands.Turma;

public record AtualizarTurmaCommand(
    Guid Id,
    string Nome,
    int CapacidadeMaxima
) : IRequest<TurmaReadDto>;