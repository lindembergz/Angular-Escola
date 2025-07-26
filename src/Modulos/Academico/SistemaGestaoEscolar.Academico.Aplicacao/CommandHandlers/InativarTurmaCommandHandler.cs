using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers;

public class InativarTurmaCommandHandler : IRequestHandler<InativarTurmaCommand>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public InativarTurmaCommandHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task Handle(InativarTurmaCommand request, CancellationToken cancellationToken)
    {
        var turma = await _repositorioTurma.ObterPorIdAsync(request.Id);
        if (turma == null)
            throw new ArgumentException("Turma não encontrada");

        turma.Inativar();
        await _repositorioTurma.AtualizarAsync(turma);
        await _repositorioTurma.SaveChangesAsync();
    }
}