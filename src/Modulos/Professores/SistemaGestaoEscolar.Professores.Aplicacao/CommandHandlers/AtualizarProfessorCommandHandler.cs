using MediatR;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Professores.Aplicacao.CommandHandlers;

public class AtualizarProfessorCommandHandler : IRequestHandler<AtualizarProfessorCommand, Unit>
{
    private readonly IRepositorioProfessor _repositorio;

    public AtualizarProfessorCommandHandler(IRepositorioProfessor repositorio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public async Task<Unit> Handle(AtualizarProfessorCommand request, CancellationToken cancellationToken)
    {
        var professor = await _repositorio.ObterPorIdAsync(request.Id);
        if (professor == null)
            throw new InvalidOperationException("Professor não encontrado");

        professor.AtualizarNome(new NomeProfessor(request.Nome));
        professor.AtualizarEmail(request.Email);
        professor.AtualizarTelefone(request.Telefone);
        professor.AtualizarObservacoes(request.Observacoes);

        await _repositorio.AtualizarAsync(professor);
        return Unit.Value;
    }
}