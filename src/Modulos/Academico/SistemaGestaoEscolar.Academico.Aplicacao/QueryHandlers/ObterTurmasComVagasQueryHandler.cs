using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers;

public class ObterTurmasComVagasQueryHandler : IRequestHandler<ObterTurmasComVagasQuery, IEnumerable<TurmaReadDto>>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ObterTurmasComVagasQueryHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task<IEnumerable<TurmaReadDto>> Handle(ObterTurmasComVagasQuery request, CancellationToken cancellationToken)
    {
        var turmas = await _repositorioTurma.ObterPorEscolaAsync(request.UnidadeEscolarId);
        
        var turmasComVagas = turmas.Where(t => t.Ativa && t.PossuiVagasDisponiveis());

        return turmasComVagas.Select(turma => new TurmaReadDto(
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
        ));
    }
}