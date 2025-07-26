using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers;

public class ObterTurmasPorAnoLetivoQueryHandler : IRequestHandler<ObterTurmasPorAnoLetivoQuery, IEnumerable<TurmaReadDto>>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ObterTurmasPorAnoLetivoQueryHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task<IEnumerable<TurmaReadDto>> Handle(ObterTurmasPorAnoLetivoQuery request, CancellationToken cancellationToken)
    {
        var turmas = await _repositorioTurma.GetAllAsync();
        
        var turmasFiltradas = turmas.Where(t => t.AnoLetivo == request.AnoLetivo);
        
        if (request.UnidadeEscolarId.HasValue)
        {
            turmasFiltradas = turmasFiltradas.Where(t => t.EscolaId == request.UnidadeEscolarId.Value);
        }

        return turmasFiltradas.Select(turma => new TurmaReadDto(
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