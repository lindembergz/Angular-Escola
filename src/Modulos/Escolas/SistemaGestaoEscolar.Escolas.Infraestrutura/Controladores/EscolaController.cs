using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Aplicacao.Servicos;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Controladores;

[ApiController]
[Route("api/[controller]")]
public class EscolaController : ControllerBase
{
    private readonly IServicoAplicacaoEscola _servicoAplicacao;
    private readonly ILogger<EscolaController> _logger;

    public EscolaController(
        IServicoAplicacaoEscola servicoAplicacao,
        ILogger<EscolaController> logger)
    {
        _servicoAplicacao = servicoAplicacao;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova escola
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EscolaRespostaDto>> CriarEscola([FromBody] CriarEscolaDto dto)
    {
        try
        {
            var escola = await _servicoAplicacao.CriarEscolaAsync(dto);
            return CreatedAtAction(nameof(ObterEscolaPorId), new { id = escola.Id }, escola);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar escola: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao criar escola: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar escola");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém uma escola por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EscolaRespostaDto>> ObterEscolaPorId(Guid id)
    {
        try
        {
            var escola = await _servicoAplicacao.ObterEscolaPorIdAsync(id);
            if (escola == null)
                return NotFound(new { message = "Escola não encontrada" });

            return Ok(escola);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter escola por ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém todas as escolas
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EscolaRespostaDto>>> ObterTodasEscolas()
    {
        try
        {
            var escolas = await _servicoAplicacao.ObterTodasEscolasAsync();
            return Ok(escolas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as escolas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém escolas ativas
    /// </summary>
    [HttpGet("ativas")]
    public async Task<ActionResult<IEnumerable<EscolaRespostaDto>>> ObterEscolasAtivas()
    {
        try
        {
            var escolas = await _servicoAplicacao.ObterEscolasAtivasAsync();
            return Ok(escolas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter escolas ativas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém escolas por tipo
    /// </summary>
    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<EscolaRespostaDto>>> ObterEscolasPorTipo(string tipo)
    {
        try
        {
            var escolas = await _servicoAplicacao.ObterEscolasPorTipoAsync(tipo);
            return Ok(escolas);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Tipo de escola inválido: {Tipo}", tipo);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter escolas por tipo: {Tipo}", tipo);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Pesquisa escolas por nome
    /// </summary>
    [HttpGet("pesquisar")]
    public async Task<ActionResult<IEnumerable<EscolaRespostaDto>>> PesquisarEscolas([FromQuery] string nome)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest(new { message = "Nome é obrigatório para pesquisa" });

            var escolas = await _servicoAplicacao.PesquisarEscolasPorNomeAsync(nome);
            return Ok(escolas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar escolas por nome: {Nome}", nome);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém escolas paginadas
    /// </summary>
    [HttpGet("paginado")]
    public async Task<ActionResult> ObterEscolasPaginado(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? filtroNome = null,
        [FromQuery] string? filtroTipo = null,
        [FromQuery] bool? filtroAtiva = null)
    {
        try
        {
            if (pagina < 1) pagina = 1;
            if (tamanhoPagina < 1 || tamanhoPagina > 100) tamanhoPagina = 10;

            var resultado = await _servicoAplicacao.ObterEscolasPaginadoAsync(
                pagina, tamanhoPagina, filtroNome, filtroTipo, filtroAtiva);

            return Ok(new
            {
                escolas = resultado.Escolas,
                total = resultado.Total,
                pagina,
                tamanhoPagina,
                totalPaginas = (int)Math.Ceiling((double)resultado.Total / tamanhoPagina)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter escolas paginadas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualiza uma escola
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EscolaRespostaDto>> AtualizarEscola(Guid id, [FromBody] CriarEscolaDto dto)
    {
        try
        {
            var escola = await _servicoAplicacao.AtualizarEscolaAsync(id, dto);
            return Ok(escola);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao atualizar escola: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Escola não encontrada para atualização: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao atualizar escola: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Ativa uma escola
    /// </summary>
    [HttpPatch("{id:guid}/ativar")]
    public async Task<ActionResult> AtivarEscola(Guid id)
    {
        try
        {
            await _servicoAplicacao.AtivarEscolaAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Escola não encontrada para ativação: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao ativar escola: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Desativa uma escola
    /// </summary>
    [HttpPatch("{id:guid}/desativar")]
    public async Task<ActionResult> DesativarEscola(Guid id)
    {
        try
        {
            await _servicoAplicacao.DesativarEscolaAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Escola não encontrada para desativação: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao desativar escola: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Remove uma escola
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoverEscola(Guid id)
    {
        try
        {
            await _servicoAplicacao.RemoverEscolaAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Escola não encontrada para remoção: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao remover escola: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém estatísticas de escolas por tipo
    /// </summary>
    [HttpGet("estatisticas/tipo")]
    public async Task<ActionResult<Dictionary<string, int>>> ObterEstatisticasPorTipo()
    {
        try
        {
            var estatisticas = await _servicoAplicacao.ObterEstatisticasEscolasPorTipoAsync();
            return Ok(estatisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas por tipo");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém estatísticas de escolas por estado
    /// </summary>
    [HttpGet("estatisticas/estado")]
    public async Task<ActionResult<Dictionary<string, int>>> ObterEstatisticasPorEstado()
    {
        try
        {
            var estatisticas = await _servicoAplicacao.ObterEstatisticasEscolasPorEstadoAsync();
            return Ok(estatisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas por estado");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém total de escolas ativas
    /// </summary>
    [HttpGet("estatisticas/total-ativas")]
    public async Task<ActionResult<int>> ObterTotalEscolasAtivas()
    {
        try
        {
            var total = await _servicoAplicacao.ObterTotalEscolasAtivasAsync();
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter total de escolas ativas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}