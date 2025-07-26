using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Controladores;

[ApiController]
[Route("api/turmas")]
[Authorize(Policy = AuthorizationPolicies.AcademicAccess)]
public class TurmaController : ControllerBase
{
    private readonly IMediator _mediator;

    public TurmaController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar uma nova turma
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CriarTurma([FromBody] CriarTurmaCommand command)
    {
        try
        {
            var turmaId = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterTurmaPorId), new { id = turmaId }, turmaId);
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
    /// Atualizar dados de uma turma
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarTurma(Guid id, [FromBody] AtualizarTurmaCommand command)
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
    /// Matricular aluno em uma turma
    /// </summary>
    [HttpPost("{id}/matricular")]
    public async Task<ActionResult> MatricularAluno(Guid id, [FromBody] MatricularAlunoCommand command)
    {
        if (id != command.TurmaId)
            return BadRequest("ID da URL não confere com o ID da turma no comando");

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Aluno matriculado com sucesso" });
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
    /// Obter lista de turmas por unidade escolar
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> ObterTurmas([FromQuery] ObterTurmasQuery query)
    {
        try
        {
            var turmas = await _mediator.Send(query);
            return Ok(turmas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter detalhes de uma turma específica
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> ObterTurmaPorId(Guid id)
    {
        try
        {
            var query = new ObterTurmaDetalheQuery { Id = id };
            var turma = await _mediator.Send(query);
            
            if (turma == null)
                return NotFound(new { message = "Turma não encontrada" });
                
            return Ok(turma);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter turmas por ano letivo
    /// </summary>
    [HttpGet("ano-letivo/{anoLetivo}")]
    public async Task<ActionResult> ObterTurmasPorAnoLetivo(int anoLetivo, [FromQuery] Guid? unidadeEscolarId = null)
    {
        try
        {
            var query = new ObterTurmasPorAnoLetivoQuery 
            { 
                AnoLetivo = anoLetivo,
                UnidadeEscolarId = unidadeEscolarId
            };
            var turmas = await _mediator.Send(query);
            return Ok(turmas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter turmas com vagas disponíveis
    /// </summary>
    [HttpGet("vagas-disponiveis")]
    public async Task<ActionResult> ObterTurmasComVagas([FromQuery] Guid unidadeEscolarId)
    {
        try
        {
            var query = new ObterTurmasComVagasQuery { UnidadeEscolarId = unidadeEscolarId };
            var turmas = await _mediator.Send(query);
            return Ok(turmas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Inativar uma turma
    /// </summary>
    [HttpPatch("{id}/inativar")]
    public async Task<ActionResult> InativarTurma(Guid id)
    {
        try
        {
            var command = new InativarTurmaCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Turma inativada com sucesso" });
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
    /// Reativar uma turma
    /// </summary>
    [HttpPatch("{id}/reativar")]
    public async Task<ActionResult> ReativarTurma(Guid id)
    {
        try
        {
            var command = new ReativarTurmaCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Turma reativada com sucesso" });
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
    /// Obter estatísticas das turmas
    /// </summary>
    [HttpGet("estatisticas")]
    public async Task<ActionResult> ObterEstatisticas([FromQuery] Guid unidadeEscolarId)
    {
        try
        {
            var query = new ObterEstatisticasTurmasQuery { UnidadeEscolarId = unidadeEscolarId };
            var estatisticas = await _mediator.Send(query);
            return Ok(estatisticas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}