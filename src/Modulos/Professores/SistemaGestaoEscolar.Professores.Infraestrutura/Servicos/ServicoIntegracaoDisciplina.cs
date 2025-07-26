using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Professores.Dominio.DTOs;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;


using MediatR;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Servicos;

/// <summary>
/// Implementação do serviço de integração com o módulo Acadêmico
/// </summary>
public class ServicoIntegracaoDisciplina : IServicoIntegracaoDisciplina
{
    private readonly IMediator _mediator;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ServicoIntegracaoDisciplina> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

    public ServicoIntegracaoDisciplina(
        IMediator mediator,
        IMemoryCache cache,
        ILogger<ServicoIntegracaoDisciplina> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> DisciplinaExisteAsync(Guid disciplinaId)
    {
        try
        {
            var cacheKey = $"disciplina_existe_{disciplinaId}";
            
            if (_cache.TryGetValue(cacheKey, out bool existe))
            {
                return existe;
            }

            var query = new ObterDisciplinaDetalheQuery { Id = disciplinaId };
            var disciplina = await _mediator.Send(query);
            
            existe = disciplina != null && disciplina.Ativa;
            
            _cache.Set(cacheKey, existe, _cacheExpiration);
            
            return existe;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se disciplina {DisciplinaId} existe", disciplinaId);
            return false;
        }
    }

    public async Task<DisciplinaInfoDto?> ObterDisciplinaAsync(Guid disciplinaId)
    {
        try
        {
            var cacheKey = $"disciplina_info_{disciplinaId}";
            
            if (_cache.TryGetValue(cacheKey, out DisciplinaInfoDto? disciplinaCache))
            {
                return disciplinaCache;
            }

            var query = new ObterDisciplinaDetalheQuery { Id = disciplinaId };
            var disciplina = await _mediator.Send(query);
            
            if (disciplina == null)
            {
                return null;
            }

            var disciplinaInfo = new DisciplinaInfoDto(
                disciplina.Id,
                disciplina.Nome,
                disciplina.Codigo,
                disciplina.CargaHoraria,
                disciplina.Serie,
                disciplina.Obrigatoria,
                disciplina.Ativa,
                disciplina.EscolaId
            );
            
            _cache.Set(cacheKey, disciplinaInfo, _cacheExpiration);
            
            return disciplinaInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações da disciplina {DisciplinaId}", disciplinaId);
            return null;
        }
    }

    public async Task<IEnumerable<DisciplinaInfoDto>> ObterDisciplinasAsync(IEnumerable<Guid> disciplinaIds)
    {
        var disciplinas = new List<DisciplinaInfoDto>();
        
        foreach (var disciplinaId in disciplinaIds)
        {
            var disciplina = await ObterDisciplinaAsync(disciplinaId);
            if (disciplina != null)
            {
                disciplinas.Add(disciplina);
            }
        }
        
        return disciplinas;
    }

    public async Task<IEnumerable<DisciplinaInfoDto>> ObterDisciplinasDisponiveisAsync(Guid escolaId)
    {
        try
        {
            var cacheKey = $"disciplinas_disponiveis_{escolaId}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<DisciplinaInfoDto>? disciplinasCache))
            {
                return disciplinasCache ?? Enumerable.Empty<DisciplinaInfoDto>();
            }

            var query = new ObterDisciplinasQuery 
            { 
                EscolaId = escolaId,
                Ativa = true,
                Page = 1,
                PageSize = 1000 // Buscar todas as disciplinas ativas
            };
            
            var response = await _mediator.Send(query);

            var disciplinas = response.Select(d => new DisciplinaInfoDto(
                d.Id,
                d.Nome,
                d.Codigo,
                d.CargaHoraria,
                d.Serie,
                d.Obrigatoria,
                d.Ativa,
                d.EscolaId
            )).ToList();
            
            _cache.Set(cacheKey, disciplinas, TimeSpan.FromMinutes(5)); // Cache menor para lista completa
            
            return disciplinas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter disciplinas disponíveis para escola {EscolaId}", escolaId);
            return Enumerable.Empty<DisciplinaInfoDto>();
        }
    }

    public async Task<bool> PodeLecionar(Guid professorId, Guid disciplinaId)
    {
        try
        {
            // Verificar se a disciplina existe e está ativa
            var disciplinaExiste = await DisciplinaExisteAsync(disciplinaId);
            if (!disciplinaExiste)
            {
                return false;
            }

            // Aqui poderiam ser implementadas regras adicionais de negócio
            // Por exemplo: verificar se o professor tem qualificação para a disciplina
            // verificar se não há conflitos de horário, etc.
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar se professor {ProfessorId} pode lecionar disciplina {DisciplinaId}", 
                professorId, disciplinaId);
            return false;
        }
    }

    /// <summary>
    /// Limpa o cache de uma disciplina específica
    /// </summary>
    public void LimparCacheDisciplina(Guid disciplinaId)
    {
        _cache.Remove($"disciplina_existe_{disciplinaId}");
        _cache.Remove($"disciplina_info_{disciplinaId}");
        
        _logger.LogDebug("Cache da disciplina {DisciplinaId} foi limpo", disciplinaId);
    }

    /// <summary>
    /// Limpa o cache de disciplinas de uma escola
    /// </summary>
    public void LimparCacheEscola(Guid escolaId)
    {
        _cache.Remove($"disciplinas_disponiveis_{escolaId}");
        
        _logger.LogDebug("Cache de disciplinas da escola {EscolaId} foi limpo", escolaId);
    }
}