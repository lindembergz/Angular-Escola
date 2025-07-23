using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Aplicacao.Servicos;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Controladores;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.NetworkManagement)]
public class RedeEscolarController : ControllerBase
{
    private readonly IServicoAplicacaoEscola _servicoAplicacao;
    private readonly ILogger<RedeEscolarController> _logger;

    public RedeEscolarController(
        IServicoAplicacaoEscola servicoAplicacao,
        ILogger<RedeEscolarController> logger)
    {
        _servicoAplicacao = servicoAplicacao;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova rede escolar
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RedeEscolarRespostaDto>> CriarRedeEscolar([FromBody] CriarRedeEscolarDto dto)
    {
        try
        {
            var rede = await _servicoAplicacao.CriarRedeEscolarAsync(dto);
            return CreatedAtAction(nameof(ObterRedeEscolarPorId), new { id = rede.Id }, rede);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar rede escolar: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao criar rede escolar: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar rede escolar");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém uma rede escolar por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RedeEscolarRespostaDto>> ObterRedeEscolarPorId(Guid id)
    {
        try
        {
            var rede = await _servicoAplicacao.ObterRedeEscolarPorIdAsync(id);
            if (rede == null)
                return NotFound(new { message = "Rede escolar não encontrada" });

            return Ok(rede);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rede escolar por ID: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém todas as redes escolares
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RedeEscolarRespostaDto>>> ObterTodasRedesEscolares()
    {
        try
        {
            var redes = await _servicoAplicacao.ObterTodasRedesEscolaresAsync();
            return Ok(redes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as redes escolares");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém redes escolares ativas
    /// </summary>
    [HttpGet("ativas")]
    public async Task<ActionResult<IEnumerable<RedeEscolarRespostaDto>>> ObterRedesEscolaresAtivas()
    {
        try
        {
            var redes = await _servicoAplicacao.ObterRedesEscolaresAtivasAsync();
            return Ok(redes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter redes escolares ativas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém redes escolares paginadas
    /// </summary>
    [HttpGet("paginado")]
    public async Task<ActionResult> ObterRedesEscolaresPaginado(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? filtroNome = null,
        [FromQuery] bool? filtroAtiva = null)
    {
        try
        {
            if (pagina < 1) pagina = 1;
            if (tamanhoPagina < 1 || tamanhoPagina > 100) tamanhoPagina = 10;

            var resultado = await _servicoAplicacao.ObterRedesEscolaresPaginadoAsync(
                pagina, tamanhoPagina, filtroNome, filtroAtiva);

            return Ok(new
            {
                redes = resultado.Redes,
                total = resultado.Total,
                pagina,
                tamanhoPagina,
                totalPaginas = (int)Math.Ceiling((double)resultado.Total / tamanhoPagina)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter redes escolares paginadas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualiza uma rede escolar
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RedeEscolarRespostaDto>> AtualizarRedeEscolar(Guid id, [FromBody] CriarRedeEscolarDto dto)
    {
        try
        {
            var rede = await _servicoAplicacao.AtualizarRedeEscolarAsync(id, dto);
            return Ok(rede);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao atualizar rede escolar: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Rede escolar não encontrada para atualização: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao atualizar rede escolar: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Ativa uma rede escolar
    /// </summary>
    [HttpPatch("{id:guid}/ativar")]
    public async Task<ActionResult> AtivarRedeEscolar(Guid id)
    {
        try
        {
            await _servicoAplicacao.AtivarRedeEscolarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Rede escolar não encontrada para ativação: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao ativar rede escolar: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Desativa uma rede escolar
    /// </summary>
    [HttpPatch("{id:guid}/desativar")]
    public async Task<ActionResult> DesativarRedeEscolar(Guid id)
    {
        try
        {
            await _servicoAplicacao.DesativarRedeEscolarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Rede escolar não encontrada para desativação: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao desativar rede escolar: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Remove uma rede escolar
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoverRedeEscolar(Guid id)
    {
        try
        {
            await _servicoAplicacao.RemoverRedeEscolarAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Rede escolar não encontrada para remoção: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao remover rede escolar: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém escolas de uma rede
    /// </summary>
    [HttpGet("{id:guid}/escolas")]
    public async Task<ActionResult<IEnumerable<EscolaRespostaDto>>> ObterEscolasDaRede(Guid id)
    {
        try
        {
            var escolas = await _servicoAplicacao.ObterEscolasPorRedeAsync(id);
            return Ok(escolas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter escolas da rede: {Id}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Associa uma escola a uma rede
    /// </summary>
    [HttpPost("{redeId:guid}/escolas/{escolaId:guid}")]
    public async Task<ActionResult> AssociarEscolaRede(Guid redeId, Guid escolaId)
    {
        try
        {
            await _servicoAplicacao.AssociarEscolaRedeAsync(escolaId, redeId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao associar escola à rede: {EscolaId} -> {RedeId}", escolaId, redeId);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao associar escola à rede: {EscolaId} -> {RedeId}", escolaId, redeId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao associar escola à rede: {EscolaId} -> {RedeId}", escolaId, redeId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Desassocia uma escola de uma rede
    /// </summary>
    [HttpDelete("{redeId:guid}/escolas/{escolaId:guid}")]
    public async Task<ActionResult> DesassociarEscolaDaRede(Guid redeId, Guid escolaId)
    {
        try
        {
            await _servicoAplicacao.DesassociarEscolaRedeAsync(escolaId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao desassociar escola da rede: {EscolaId}", escolaId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao desassociar escola da rede: {EscolaId}", escolaId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Adiciona uma unidade escolar
    /// </summary>
    [HttpPost("unidades")]
    public async Task<ActionResult<UnidadeEscolarRespostaDto>> AdicionarUnidadeEscolar([FromBody] AdicionarUnidadeEscolarDto dto)
    {
        try
        {
            var unidade = await _servicoAplicacao.AdicionarUnidadeEscolarAsync(dto);
            return CreatedAtAction(nameof(ObterRedeEscolarPorId), new { id = dto.EscolaId }, unidade);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao adicionar unidade escolar: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de operação ao adicionar unidade escolar: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao adicionar unidade escolar");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Remove uma unidade escolar
    /// </summary>
    [HttpDelete("escolas/{escolaId:guid}/unidades/{unidadeId:guid}")]
    public async Task<ActionResult> RemoverUnidadeEscolar(Guid escolaId, Guid unidadeId)
    {
        try
        {
            await _servicoAplicacao.RemoverUnidadeEscolarAsync(escolaId, unidadeId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao remover unidade escolar: {EscolaId}/{UnidadeId}", escolaId, unidadeId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao remover unidade escolar: {EscolaId}/{UnidadeId}", escolaId, unidadeId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém total de redes escolares ativas
    /// </summary>
    [HttpGet("estatisticas/total-ativas")]
    public async Task<ActionResult<int>> ObterTotalRedesEscolaresAtivas()
    {
        try
        {
            var total = await _servicoAplicacao.ObterTotalRedesEscolaresAtivasAsync();
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter total de redes escolares ativas");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}