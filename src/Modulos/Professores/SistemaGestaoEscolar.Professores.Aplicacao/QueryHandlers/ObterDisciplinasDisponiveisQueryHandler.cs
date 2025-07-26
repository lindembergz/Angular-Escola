using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Professores.Aplicacao.Queries;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;

namespace SistemaGestaoEscolar.Professores.Aplicacao.QueryHandlers;

public class ObterDisciplinasDisponiveisQueryHandler : IRequestHandler<ObterDisciplinasDisponiveisQuery, IEnumerable<DisciplinaDisponivelDto>>
{
    private readonly IServicoIntegracaoDisciplina _servicoIntegracaoDisciplina;
    private readonly ILogger<ObterDisciplinasDisponiveisQueryHandler> _logger;

    public ObterDisciplinasDisponiveisQueryHandler(
        IServicoIntegracaoDisciplina servicoIntegracaoDisciplina,
        ILogger<ObterDisciplinasDisponiveisQueryHandler> logger)
    {
        _servicoIntegracaoDisciplina = servicoIntegracaoDisciplina ?? throw new ArgumentNullException(nameof(servicoIntegracaoDisciplina));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<DisciplinaDisponivelDto>> Handle(ObterDisciplinasDisponiveisQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Buscando disciplinas disponíveis para escola {EscolaId}", request.EscolaId);

            var disciplinas = await _servicoIntegracaoDisciplina.ObterDisciplinasDisponiveisAsync(request.EscolaId);

            var disciplinasDto = disciplinas.Select(d => new DisciplinaDisponivelDto(
                d.Id,
                d.Nome,
                d.Codigo,
                d.CargaHoraria,
                d.Serie,
                d.Obrigatoria
            )).ToList();

            _logger.LogInformation("Encontradas {Count} disciplinas disponíveis para escola {EscolaId}", 
                disciplinasDto.Count, request.EscolaId);

            return disciplinasDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar disciplinas disponíveis para escola {EscolaId}", request.EscolaId);
            throw;
        }
    }
}