using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers;

public class ReativarTurmaCommandHandler : IRequestHandler<ReativarTurmaCommand>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ReativarTurmaCommandHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task Handle(ReativarTurmaCommand request, CancellationToken cancellationToken)
    {
        var turma = await _repositorioTurma.ObterPorIdAsync(request.Id);
        if (turma == null)
            throw new ArgumentException("Turma não encontrada");

        turma.Reativar();
        await _repositorioTurma.AtualizarAsync(turma);
        await _repositorioTurma.SaveChangesAsync();
    }
}