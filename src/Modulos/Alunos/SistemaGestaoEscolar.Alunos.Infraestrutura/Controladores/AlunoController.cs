using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Aplicacao.DTOs;
using SistemaGestaoEscolar.Alunos.Aplicacao.Queries;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Controladores;

[ApiController]
[Route("api/alunos")]
[Authorize(Policy = AuthorizationPolicies.StudentManagement)]
public class AlunoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<MatricularAlunoRequest> _matricularAlunoRequestValidator;

    public AlunoController(IMediator mediator, IValidator<MatricularAlunoRequest> matricularAlunoRequestValidator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _matricularAlunoRequestValidator = matricularAlunoRequestValidator ?? throw new ArgumentNullException(nameof(matricularAlunoRequestValidator));
    }

    /// <summary>
    /// Criar um novo aluno
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CriarAlunoResponse>> CriarAluno([FromBody] CriarAlunoCommand command)
    {
        var response = await _mediator.Send(command);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return CreatedAtAction(nameof(ObterAlunoPorId), new { id = response.Id }, response);
    }

    /// <summary>
    /// Atualizar dados de um aluno
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AtualizarAlunoResponse>> AtualizarAluno(Guid id, [FromBody] AtualizarAlunoCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID da URL não confere com o ID do comando");

        var response = await _mediator.Send(command);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Transferir aluno para outra escola
    /// </summary>
    [HttpPost("{id}/transferir")]
    public async Task<ActionResult<TransferirAlunoResponse>> TransferirAluno(Guid id, [FromBody] TransferirAlunoCommand command)
    {
        if (id != command.AlunoId)
            return BadRequest("ID da URL não confere com o ID do comando");

        var response = await _mediator.Send(command);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Matricular aluno em uma turma
    /// </summary>
    [HttpPost("{id}/matricular")]
    public async Task<ActionResult<MatricularAlunoResponse>> MatricularAluno(Guid id, [FromBody] MatricularAlunoRequest request)
    {
        // Validar o request usando FluentValidation
        var validationResult = await _matricularAlunoRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new MatricularAlunoResponse
            {
                Sucesso = false,
                Erros = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        // Converter string para Guid (já validado pelo validator)
        var turmaGuid = Guid.Parse(request.TurmaId);

        // Converter request para command
        var command = new MatricularAlunoCommand
        {
            AlunoId = id,
            TurmaId = turmaGuid,
            AnoLetivo = request.AnoLetivo,
            Observacoes = request.Observacoes
        };

        var response = await _mediator.Send(command);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Obter lista de alunos com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ObterAlunosResponse>> ObterAlunos([FromQuery] ObterAlunosQuery query)
    {
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Obter detalhes completos de um aluno
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ObterAlunoDetalheResponse>> ObterAlunoPorId(string id)
    {
        // Validar se o ID é um GUID válido
        if (!Guid.TryParse(id, out Guid guidId))
        {
            return BadRequest(new ObterAlunoDetalheResponse
            {
                Sucesso = false,
                Erros = new List<string> { $"ID inválido. O ID deve ser um GUID válido. Valor recebido: '{id}'" }
            });
        }

        var query = new ObterAlunoDetalheQuery { Id = guidId };
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
        {
            if (response.Erros.Any(e => e.Contains("não encontrado")))
                return NotFound(response);
            return BadRequest(response);
        }
            
        return Ok(response);
    }

    /// <summary>
    /// Obter alunos de uma turma específica
    /// </summary>
    [HttpGet("turma/{turmaId}")]
    public async Task<ActionResult<ObterAlunosPorTurmaResponse>> ObterAlunosPorTurma(Guid turmaId, [FromQuery] ObterAlunosPorTurmaQuery query)
    {
        query.TurmaId = turmaId;
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Obter detalhes de um aluno incluindo responsáveis e matrículas
    /// </summary>
    [HttpGet("{id}/completo")]
    public async Task<ActionResult<ObterAlunoDetalheResponse>> ObterAlunoCompleto(Guid id)
    {
        var query = new ObterAlunoDetalheQuery 
        { 
            Id = id, 
            IncluirResponsaveis = true, 
            IncluirMatriculas = true 
        };
        
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
        {
            if (response.Erros.Any(e => e.Contains("não encontrado")))
                return NotFound(response);
            return BadRequest(response);
        }
            
        return Ok(response);
    }

    /// <summary>
    /// Buscar alunos por nome
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<ObterAlunosResponse>> BuscarAlunos([FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new ObterAlunosQuery 
        { 
            Name = name,
            Page = page,
            PageSize = pageSize
        };
        
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Obter alunos por escola
    /// </summary>
    [HttpGet("escola/{escolaId}")]
    public async Task<ActionResult<ObterAlunosResponse>> ObterAlunosPorEscola(Guid escolaId, [FromQuery] ObterAlunosQuery query)
    {
        query.SchoolId = escolaId;
        var response = await _mediator.Send(query);
        
        if (!response.Sucesso)
            return BadRequest(response);
            
        return Ok(response);
    }

    /// <summary>
    /// Obter opções de gênero e deficiência para formulários
    /// </summary>
    [HttpGet("opcoes")]
    [AllowAnonymous]
    public ActionResult<OpcoesAlunoResponse> ObterOpcoes()
    {
        var response = new OpcoesAlunoResponse
        {
            Generos = new List<OpcaoDto>
            {
                new() { Valor = 0, Descricao = "Não Informado" },
                new() { Valor = 1, Descricao = "Masculino" },
                new() { Valor = 2, Descricao = "Feminino" },
                new() { Valor = 3, Descricao = "Não Binário" }
            },
            TiposDeficiencia = new List<OpcaoDto>
            {
                new() { Valor = 1, Descricao = "Visual" },
                new() { Valor = 2, Descricao = "Auditiva" },
                new() { Valor = 3, Descricao = "Física" },
                new() { Valor = 4, Descricao = "Intelectual" },
                new() { Valor = 5, Descricao = "Múltipla" },
                new() { Valor = 6, Descricao = "Autismo" },
                new() { Valor = 7, Descricao = "Surdocegueira" }
            }
        };
        
        return Ok(response);
    }
}

public class OpcoesAlunoResponse
{
    public List<OpcaoDto> Generos { get; set; } = new();
    public List<OpcaoDto> TiposDeficiencia { get; set; } = new();
}

public class OpcaoDto
{
    public int Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
}