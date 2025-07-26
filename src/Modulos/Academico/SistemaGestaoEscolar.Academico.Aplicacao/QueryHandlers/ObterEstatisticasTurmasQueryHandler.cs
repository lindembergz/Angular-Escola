using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers;

public class ObterEstatisticasTurmasQueryHandler : IRequestHandler<ObterEstatisticasTurmasQuery, Dictionary<string, int>>
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ObterEstatisticasTurmasQueryHandler(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task<Dictionary<string, int>> Handle(ObterEstatisticasTurmasQuery request, CancellationToken cancellationToken)
    {
        var turmas = await _repositorioTurma.ObterPorEscolaAsync(request.UnidadeEscolarId);
        
        var estatisticas = new Dictionary<string, int>
        {
            ["Total"] = turmas.Count(),
            ["Ativas"] = turmas.Count(t => t.Ativa),
            ["Inativas"] = turmas.Count(t => !t.Ativa),
            ["Matutino"] = turmas.Count(t => t.Turno.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoTurno.Matutino),
            ["Vespertino"] = turmas.Count(t => t.Turno.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoTurno.Vespertino),
            ["Noturno"] = turmas.Count(t => t.Turno.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoTurno.Noturno),
            ["Integral"] = turmas.Count(t => t.Turno.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoTurno.Integral),
            ["Infantil"] = turmas.Count(t => t.Serie.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoSerie.Infantil),
            ["Fundamental"] = turmas.Count(t => t.Serie.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoSerie.Fundamental),
            ["Médio"] = turmas.Count(t => t.Serie.Tipo == SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor.TipoSerie.Medio)
        };

        return estatisticas;
    }
}