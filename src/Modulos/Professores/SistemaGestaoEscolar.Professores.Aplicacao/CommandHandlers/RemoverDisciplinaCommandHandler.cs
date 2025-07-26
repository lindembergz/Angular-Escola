using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Professores.Aplicacao.CommandHandlers;

public class RemoverDisciplinaCommandHandler : IRequestHandler<RemoverDisciplinaCommand, Unit>
{
    private readonly IRepositorioProfessor _repositorio;

    public RemoverDisciplinaCommandHandler(IRepositorioProfessor repositorio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public async Task<Unit> Handle(RemoverDisciplinaCommand request, CancellationToken cancellationToken)
    {
        var professor = await _repositorio.ObterPorIdAsync(request.ProfessorId);
        if (professor == null)
            throw new InvalidOperationException("Professor não encontrado");

        professor.RemoverDisciplina(request.DisciplinaId);

        await _repositorio.AtualizarAsync(professor);
        return Unit.Value;
    }
}