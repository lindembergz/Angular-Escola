using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Aplicacao.Queries;
using SistemaGestaoEscolar.Professores.Infraestrutura.DTOs;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Controladores;

[ApiController]
[Route("api/professores")]
[Authorize]
public class ProfessorController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfessorController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Criar um novo professor
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CriarProfessor([FromBody] CriarProfessorCommand command)
    {
        try
        {
            var professorId = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterProfessorPorId), new { id = professorId }, professorId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar dados de um professor
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarProfessor(Guid id, [FromBody] AtualizarProfessorCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "ID da URL não confere com o ID do comando" });

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Desativar um professor
    /// </summary>
    [HttpPost("{id}/desativar")]
    public async Task<ActionResult> DesativarProfessor(Guid id, [FromBody] DesativarProfessorCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { error = "ID da URL não confere com o ID do comando" });

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Atribuir disciplina a um professor
    /// </summary>
    [HttpPost("{id}/disciplinas")]
    public async Task<ActionResult> AtribuirDisciplina(Guid id, [FromBody] AtribuirDisciplinaRequest request)
    {
        try
        {
            var command = new AtribuirDisciplinaCommand(
                id, 
                request.DisciplinaId, 
                request.CargaHorariaSemanal, 
                request.Observacoes);

            await _mediator.Send(command);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remover disciplina de um professor
    /// </summary>
    [HttpDelete("{id}/disciplinas/{disciplinaId}")]
    public async Task<ActionResult> RemoverDisciplina(Guid id, Guid disciplinaId)
    {
        var command = new RemoverDisciplinaCommand(id, disciplinaId);

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obter lista de professores com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProfessorResumoDto>>> ListarProfessores([FromQuery] ListarProfessoresQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter detalhes completos de um professor
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessorDto>> ObterProfessorPorId(Guid id)
    {
        var query = new ObterProfessorPorIdQuery(id);
        var professor = await _mediator.Send(query);

        if (professor == null)
            return NotFound(new { error = $"Professor com ID {id} não encontrado" });

        return Ok(professor);
    }

    /// <summary>
    /// Obter professores por escola
    /// </summary>
    [HttpGet("escola/{escolaId}")]
    public async Task<ActionResult<PaginatedResult<ProfessorResumoDto>>> ObterProfessoresPorEscola(
        Guid escolaId, 
        [FromQuery] int pagina = 1, 
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] bool? ativo = null,
        [FromQuery] string? nome = null)
    {
        var query = new ListarProfessoresQuery(escolaId, ativo, nome, pagina, tamanhoPagina);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter professores ativos
    /// </summary>
    [HttpGet("ativos")]
    public async Task<ActionResult<PaginatedResult<ProfessorResumoDto>>> ObterProfessoresAtivos(
        [FromQuery] Guid? escolaId = null,
        [FromQuery] int pagina = 1, 
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? nome = null)
    {
        var query = new ListarProfessoresQuery(escolaId, true, nome, pagina, tamanhoPagina);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Buscar professores por nome
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<PaginatedResult<ProfessorResumoDto>>> BuscarProfessores(
        [FromQuery] string nome,
        [FromQuery] Guid? escolaId = null,
        [FromQuery] int pagina = 1, 
        [FromQuery] int tamanhoPagina = 10)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest(new { error = "Nome é obrigatório para busca" });

        var query = new ListarProfessoresQuery(escolaId, null, nome, pagina, tamanhoPagina);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obter opções para formulários (tipos de título, etc.)
    /// </summary>
    [HttpGet("opcoes")]
    [AllowAnonymous]
    public ActionResult<OpcoesProfessorResponse> ObterOpcoes()
    {
        var response = new OpcoesProfessorResponse
        {
            TiposTitulo = new List<OpcaoDto>
            {
                new() { Valor = 1, Descricao = "Ensino Médio" },
                new() { Valor = 2, Descricao = "Tecnólogo" },
                new() { Valor = 3, Descricao = "Graduação" },
                new() { Valor = 4, Descricao = "Pós-Graduação" },
                new() { Valor = 5, Descricao = "Mestrado" },
                new() { Valor = 6, Descricao = "Doutorado" },
                new() { Valor = 7, Descricao = "Pós-Doutorado" }
            }
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Debug: Verificar claims do usuário autenticado
    /// </summary>
    [HttpGet("debug/claims")]
    [Authorize]
    public ActionResult<object> DebugClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        var isInTeacherRole = User.IsInRole("Teacher");
        var isInAdminRole = User.IsInRole("Admin");
        var isInDirectorRole = User.IsInRole("Director");
        
        return Ok(new
        {
            Claims = claims,
            Roles = roles,
            IsInTeacherRole = isInTeacherRole,
            IsInAdminRole = isInAdminRole,
            IsInDirectorRole = isInDirectorRole,
            Identity = new
            {
                Name = User.Identity?.Name,
                IsAuthenticated = User.Identity?.IsAuthenticated,
                AuthenticationType = User.Identity?.AuthenticationType
            }
        });
    }

    /// <summary>
    /// Obter disciplinas disponíveis para atribuição a professores
    /// </summary>
    [HttpGet("disciplinas-disponiveis/{escolaId}")]
    public async Task<ActionResult<IEnumerable<DisciplinaDisponivelDto>>> ObterDisciplinasDisponiveis(Guid escolaId)
    {
        try
        {
            var query = new ObterDisciplinasDisponiveisQuery(escolaId);
            var disciplinas = await _mediator.Send(query);
            return Ok(disciplinas);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obter estatísticas de professores por escola
    /// </summary>
    [HttpGet("escola/{escolaId}/estatisticas")]
    public async Task<ActionResult<EstatisticasProfessorResponse>> ObterEstatisticas(Guid escolaId)
    {
        // This would typically be implemented with specific queries for statistics
        // For now, we'll use the basic listing to provide some stats
        var professoresAtivos = await _mediator.Send(new ListarProfessoresQuery(escolaId, true, null, 1, 1000));
        var professoresInativos = await _mediator.Send(new ListarProfessoresQuery(escolaId, false, null, 1, 1000));

        var response = new EstatisticasProfessorResponse
        {
            TotalProfessores = professoresAtivos.TotalItems + professoresInativos.TotalItems,
            ProfessoresAtivos = professoresAtivos.TotalItems,
            ProfessoresInativos = professoresInativos.TotalItems,
            CargaHorariaMedia = professoresAtivos.Items.Any() 
                ? (int)professoresAtivos.Items.Average(p => p.CargaHorariaTotal) 
                : 0,
            ProfessoresComTituloSuperior = professoresAtivos.Items.Count(p => 
                p.MaiorTitulo.Contains("Graduação") || 
                p.MaiorTitulo.Contains("Pós-Graduação") || 
                p.MaiorTitulo.Contains("Mestrado") || 
                p.MaiorTitulo.Contains("Doutorado"))
        };

        return Ok(response);
    }
}

public class OpcoesProfessorResponse
{
    public List<OpcaoDto> TiposTitulo { get; set; } = new();
}

public class OpcaoDto
{
    public int Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class EstatisticasProfessorResponse
{
    public int TotalProfessores { get; set; }
    public int ProfessoresAtivos { get; set; }
    public int ProfessoresInativos { get; set; }
    public int CargaHorariaMedia { get; set; }
    public int ProfessoresComTituloSuperior { get; set; }
}