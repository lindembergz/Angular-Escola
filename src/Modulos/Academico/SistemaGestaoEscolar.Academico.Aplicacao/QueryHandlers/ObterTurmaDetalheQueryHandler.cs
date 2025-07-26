using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers;

public class ObterTurmaDetalheQueryHandler : IRequestHandler<ObterTurmaDetalheQuery, TurmaReadDto?>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ObterTurmaDetalheQueryHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task<TurmaReadDto?> Handle(ObterTurmaDetalheQuery request, CancellationToken cancellationToken)
    {
        var turma = await _repositorioTurma.ObterPorIdAsync(request.Id);
        if (turma == null)
            return null;

        return new TurmaReadDto(
            turma.Id,
            turma.Nome.Valor,
            turma.Serie.Descricao,
            turma.Turno.Descricao,
            turma.CapacidadeMaxima,
            turma.AnoLetivo,
            turma.EscolaId,
            turma.Ativa,
            turma.AlunosMatriculados.Count,
            turma.ObterVagasDisponiveis(),
            turma.CreatedAt
        );
    }
}