using MediatR;

namespace SistemaGestaoEscolar.Professores.Aplicacao.Commands;

public record AtribuirDisciplinaCommand(
    Guid ProfessorId,
    Guid DisciplinaId,
    int CargaHorariaSemanal,
    string? Observacoes = null) : IRequest<Unit>;