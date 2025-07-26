using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers;

public class AtualizarDisciplinaCommandHandler : IRequestHandler<AtualizarDisciplinaCommand>
{
    private readonly IRepositorioDisciplina _repositorioDisciplina;

    public AtualizarDisciplinaCommandHandler(IRepositorioDisciplina repositorioDisciplina)
    {
        _repositorioDisciplina = repositorioDisciplina;
    }

    public async Task Handle(AtualizarDisciplinaCommand request, CancellationToken cancellationToken)
    {
        var disciplina = await _repositorioDisciplina.ObterPorIdAsync(request.Id);
        
        if (disciplina == null)
            throw new InvalidOperationException("Disciplina não encontrada");

        disciplina.AtualizarInformacoes(
            request.Nome,
            request.CargaHoraria,
            request.Obrigatoria,
            request.Descricao
        );

        await _repositorioDisciplina.UpdateAsync(disciplina);
        await _repositorioDisciplina.SaveChangesAsync();
    }
}