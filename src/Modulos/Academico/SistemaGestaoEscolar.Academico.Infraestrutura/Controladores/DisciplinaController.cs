using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Controladores;

[ApiController]
[Route("api/disciplinas")]
[Authorize(Policy = AuthorizationPolicies.AcademicAccess)]
public class DisciplinaController : ControllerBase
{
    private readonly IMediator _mediator;

    public DisciplinaController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar uma nova disciplina
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CriarDisciplina([FromBody] CriarDisciplinaCommand command)
    {
        try
        {
            var disciplinaId = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterDisciplinaPorId), new { id = disciplinaId }, disciplinaId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar dados de uma disciplina
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarDisciplina(Guid id, [FromBody] AtualizarDisciplinaCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID da URL não confere com o ID do comando");

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter lista de disciplinas por escola
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> ObterDisciplinas([FromQuery] ObterDisciplinasQuery query)
    {
        try
        {
            var disciplinas = await _mediator.Send(query);
            return Ok(disciplinas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter detalhes de uma disciplina específica
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> ObterDisciplinaPorId(Guid id)
    {
        try
        {
            var query = new ObterDisciplinaDetalheQuery { Id = id };
            var disciplina = await _mediator.Send(query);
            
            if (disciplina == null)
                return NotFound(new { message = "Disciplina não encontrada" });
                
            return Ok(disciplina);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter disciplinas por série
    /// </summary>
    [HttpGet("serie")]
    public async Task<ActionResult> ObterDisciplinasPorSerie([FromQuery] ObterDisciplinasPorSerieQuery query)
    {
        try
        {
            var disciplinas = await _mediator.Send(query);
            return Ok(disciplinas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter disciplinas obrigatórias
    /// </summary>
    [HttpGet("obrigatorias")]
    public async Task<ActionResult> ObterDisciplinasObrigatorias([FromQuery] Guid escolaId)
    {
        try
        {
            var query = new ObterDisciplinasObrigatoriasQuery { EscolaId = escolaId };
            var disciplinas = await _mediator.Send(query);
            return Ok(disciplinas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Pesquisar disciplinas por nome
    /// </summary>
    [HttpGet("pesquisar")]
    public async Task<ActionResult> PesquisarDisciplinas([FromQuery] string nome, [FromQuery] Guid escolaId)
    {
        try
        {
            var query = new PesquisarDisciplinasQuery { Nome = nome, EscolaId = escolaId };
            var disciplinas = await _mediator.Send(query);
            return Ok(disciplinas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Adicionar pré-requisito a uma disciplina
    /// </summary>
    [HttpPost("{id}/pre-requisitos")]
    public async Task<ActionResult> AdicionarPreRequisito(Guid id, [FromBody] AdicionarPreRequisitoCommand command)
    {
        if (id != command.DisciplinaId)
            return BadRequest("ID da URL não confere com o ID da disciplina no comando");

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Pré-requisito adicionado com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remover pré-requisito de uma disciplina
    /// </summary>
    [HttpDelete("{id}/pre-requisitos/{preRequisitoId}")]
    public async Task<ActionResult> RemoverPreRequisito(Guid id, Guid preRequisitoId)
    {
        try
        {
            var command = new RemoverPreRequisitoCommand 
            { 
                DisciplinaId = id, 
                PreRequisitoId = preRequisitoId 
            };
            await _mediator.Send(command);
            return Ok(new { message = "Pré-requisito removido com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Inativar uma disciplina
    /// </summary>
    [HttpPatch("{id}/inativar")]
    public async Task<ActionResult> InativarDisciplina(Guid id)
    {
        try
        {
            var command = new InativarDisciplinaCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Disciplina inativada com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Reativar uma disciplina
    /// </summary>
    [HttpPatch("{id}/reativar")]
    public async Task<ActionResult> ReativarDisciplina(Guid id)
    {
        try
        {
            var command = new ReativarDisciplinaCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Disciplina reativada com sucesso" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter estatísticas das disciplinas
    /// </summary>
    [HttpGet("estatisticas")]
    public async Task<ActionResult> ObterEstatisticas([FromQuery] Guid escolaId)
    {
        try
        {
            var query = new ObterEstatisticasDisciplinasQuery { EscolaId = escolaId };
            var estatisticas = await _mediator.Send(query);
            return Ok(estatisticas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}