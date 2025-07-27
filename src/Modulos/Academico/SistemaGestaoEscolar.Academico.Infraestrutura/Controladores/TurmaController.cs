using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
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
    public async Task<ActionResult<Guid>> CriarTurma([FromBody] TurmaCreateDto request)
    {
        try
        {
            var command = MapearParaCommand(request);
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
    /// Obter lista de turmas com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> ObterTurmas(
        [FromQuery] Guid? escolaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? anoLetivo = null,
        [FromQuery] string? serie = null,
        [FromQuery] string? turno = null,
        [FromQuery] bool? ativa = null)
    {
        try
        {
            var query = new ObterTurmasQuery
            {
                EscolaId = escolaId,
                Page = page,
                PageSize = pageSize,
                AnoLetivo = anoLetivo,
                Serie = serie,
                Turno = turno,
                Ativa = ativa
            };

            var response = await _mediator.Send(query);
            return Ok(response);
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

    private CriarTurmaCommand MapearParaCommand(TurmaCreateDto request)
    {
        var (tipoSerie, anoSerie) = ParsearSerie(request.Serie);
        var tipoTurno = ParsearTurno(request.Turno);

        return new CriarTurmaCommand
        {
            Nome = request.Nome,
            TipoSerie = tipoSerie,
            AnoSerie = anoSerie,
            TipoTurno = tipoTurno,
            HoraInicioTurno = ObterHoraInicioTurno(tipoTurno),
            HoraFimTurno = ObterHoraFimTurno(tipoTurno),
            CapacidadeMaxima = request.CapacidadeMaxima,
            AnoLetivo = request.AnoLetivo,
            UnidadeEscolarId = request.EscolaId
        };
    }

    private (TipoSerie tipoSerie, int anoSerie) ParsearSerie(string serie)
    {
        return serie.ToLower() switch
        {
            "1º ano" or "1° ano" => (TipoSerie.Fundamental, 1),
            "2º ano" or "2° ano" => (TipoSerie.Fundamental, 2),
            "3º ano" or "3° ano" => (TipoSerie.Fundamental, 3),
            "4º ano" or "4° ano" => (TipoSerie.Fundamental, 4),
            "5º ano" or "5° ano" => (TipoSerie.Fundamental, 5),
            "6º ano" or "6° ano" => (TipoSerie.Fundamental, 6),
            "7º ano" or "7° ano" => (TipoSerie.Fundamental, 7),
            "8º ano" or "8° ano" => (TipoSerie.Fundamental, 8),
            "9º ano" or "9° ano" => (TipoSerie.Fundamental, 9),
            "1º em" or "1° em" => (TipoSerie.Medio, 1),
            "2º em" or "2° em" => (TipoSerie.Medio, 2),
            "3º em" or "3° em" => (TipoSerie.Medio, 3),
            _ => throw new ArgumentException($"Tipo de série inválido: {serie}")
        };
    }

    private TipoTurno ParsearTurno(string turno)
    {
        return turno.ToLower() switch
        {
            "matutino" => TipoTurno.Matutino,
            "vespertino" => TipoTurno.Vespertino,
            "noturno" => TipoTurno.Noturno,
            "integral" => TipoTurno.Integral,
            _ => throw new ArgumentException($"Tipo de turno inválido: {turno}")
        };
    }

    private TimeOnly ObterHoraInicioTurno(TipoTurno tipoTurno)
    {
        return tipoTurno switch
        {
            TipoTurno.Matutino => new TimeOnly(7, 0),
            TipoTurno.Vespertino => new TimeOnly(13, 0),
            TipoTurno.Noturno => new TimeOnly(19, 0),
            TipoTurno.Integral => new TimeOnly(7, 0),
            _ => new TimeOnly(7, 0)
        };
    }

    private TimeOnly ObterHoraFimTurno(TipoTurno tipoTurno)
    {
        return tipoTurno switch
        {
            TipoTurno.Matutino => new TimeOnly(12, 0),
            TipoTurno.Vespertino => new TimeOnly(18, 0),
            TipoTurno.Noturno => new TimeOnly(22, 0),
            TipoTurno.Integral => new TimeOnly(17, 0),
            _ => new TimeOnly(12, 0)
        };
    }
}