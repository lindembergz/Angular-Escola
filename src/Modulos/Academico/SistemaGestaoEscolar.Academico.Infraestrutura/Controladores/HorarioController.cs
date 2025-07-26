using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Controladores;

[ApiController]
[Route("api/horarios")]
[Authorize(Policy = AuthorizationPolicies.AcademicAccess)]
public class HorarioController : ControllerBase
{
    private readonly IMediator _mediator;

    public HorarioController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar um novo horário
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CriarHorario([FromBody] CriarHorarioCommand command)
    {
        try
        {
            var horarioId = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterHorarioPorId), new { id = horarioId }, horarioId);
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
    /// Atualizar dados de um horário
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarHorario(Guid id, [FromBody] AtualizarHorarioCommand command)
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
    /// Obter detalhes de um horário específico
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> ObterHorarioPorId(Guid id)
    {
        try
        {
            var query = new ObterHorarioDetalheQuery { Id = id };
            var horario = await _mediator.Send(query);
            
            if (horario == null)
                return NotFound(new { message = "Horário não encontrado" });
                
            return Ok(horario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter grade horária de uma turma
    /// </summary>
    [HttpGet("turma/{turmaId}")]
    public async Task<ActionResult> ObterGradeHorariaTurma(Guid turmaId, [FromQuery] int anoLetivo, [FromQuery] int semestre)
    {
        try
        {
            var query = new ObterGradeHorariaQuery 
            { 
                TurmaId = turmaId, 
                AnoLetivo = anoLetivo, 
                Semestre = semestre 
            };
            var gradeHoraria = await _mediator.Send(query);
            return Ok(gradeHoraria);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter horários de um professor
    /// </summary>
    [HttpGet("professor/{professorId}")]
    public async Task<ActionResult> ObterHorariosProfessor(Guid professorId, [FromQuery] int anoLetivo, [FromQuery] int semestre)
    {
        try
        {
            var query = new ObterHorariosProfessorQuery 
            { 
                ProfessorId = professorId, 
                AnoLetivo = anoLetivo, 
                Semestre = semestre 
            };
            var horarios = await _mediator.Send(query);
            return Ok(horarios);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter horários de uma disciplina
    /// </summary>
    [HttpGet("disciplina/{disciplinaId}")]
    public async Task<ActionResult> ObterHorariosDisciplina(Guid disciplinaId, [FromQuery] int anoLetivo, [FromQuery] int semestre)
    {
        try
        {
            var query = new ObterHorariosDisciplinaQuery 
            { 
                DisciplinaId = disciplinaId, 
                AnoLetivo = anoLetivo, 
                Semestre = semestre 
            };
            var horarios = await _mediator.Send(query);
            return Ok(horarios);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter horários por dia da semana
    /// </summary>
    [HttpGet("dia-semana/{diaSemana}")]
    public async Task<ActionResult> ObterHorariosPorDia(DayOfWeek diaSemana, [FromQuery] Guid? escolaId = null)
    {
        try
        {
            var query = new ObterHorariosPorDiaQuery 
            { 
                DiaSemana = diaSemana,
                EscolaId = escolaId
            };
            var horarios = await _mediator.Send(query);
            return Ok(horarios);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verificar conflitos de horário
    /// </summary>
    [HttpPost("verificar-conflitos")]
    public async Task<ActionResult> VerificarConflitos([FromBody] VerificarConflitosHorarioQuery query)
    {
        try
        {
            var conflitos = await _mediator.Send(query);
            return Ok(conflitos);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar um horário
    /// </summary>
    [HttpPatch("{id}/cancelar")]
    public async Task<ActionResult> CancelarHorario(Guid id)
    {
        try
        {
            var command = new CancelarHorarioCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Horário cancelado com sucesso" });
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
    /// Reativar um horário
    /// </summary>
    [HttpPatch("{id}/reativar")]
    public async Task<ActionResult> ReativarHorario(Guid id)
    {
        try
        {
            var command = new ReativarHorarioCommand { Id = id };
            await _mediator.Send(command);
            return Ok(new { message = "Horário reativado com sucesso" });
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
    /// Alterar professor de um horário
    /// </summary>
    [HttpPatch("{id}/alterar-professor")]
    public async Task<ActionResult> AlterarProfessor(Guid id, [FromBody] AlterarProfessorHorarioCommand command)
    {
        if (id != command.HorarioId)
            return BadRequest("ID da URL não confere com o ID do horário no comando");

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Professor alterado com sucesso" });
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
    /// Alterar sala de um horário
    /// </summary>
    [HttpPatch("{id}/alterar-sala")]
    public async Task<ActionResult> AlterarSala(Guid id, [FromBody] AlterarSalaHorarioCommand command)
    {
        if (id != command.HorarioId)
            return BadRequest("ID da URL não confere com o ID do horário no comando");

        try
        {
            await _mediator.Send(command);
            return Ok(new { message = "Sala alterada com sucesso" });
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
    /// Obter estatísticas de horários
    /// </summary>
    [HttpGet("estatisticas")]
    public async Task<ActionResult> ObterEstatisticas([FromQuery] Guid escolaId, [FromQuery] int anoLetivo, [FromQuery] int semestre)
    {
        try
        {
            var query = new ObterEstatisticasHorariosQuery 
            { 
                EscolaId = escolaId, 
                AnoLetivo = anoLetivo, 
                Semestre = semestre 
            };
            var estatisticas = await _mediator.Send(query);
            return Ok(estatisticas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}