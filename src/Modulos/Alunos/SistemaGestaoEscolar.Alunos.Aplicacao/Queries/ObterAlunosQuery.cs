using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Queries;

public class ObterAlunosQuery : IRequest<ObterAlunosResponse>
{
    // Parâmetros em inglês para compatibilidade com frontend
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Name { get; set; }
    public Guid? SchoolId { get; set; }
    public bool? Active { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool? HasActiveEnrollment { get; set; }
    
    // Propriedades internas em português para compatibilidade com o handler
    public int Pagina => Page;
    public int TamanhoPagina => PageSize;
    public string? FiltroNome => Name;
    public Guid? FiltroEscola => SchoolId;
    public bool? FiltroAtivo => Active;
    public int? IdadeMinima => MinAge;
    public int? IdadeMaxima => MaxAge;
    public string? FiltroCidade => City;
    public string? FiltroEstado => State;
    public bool? PossuiMatriculaAtiva => HasActiveEnrollment;
}

public class ObterAlunosResponse
{
    public List<AlunoResumoDto> Alunos { get; set; } = new();
    public int TotalRegistros { get; set; }
    public int TotalPaginas { get; set; }
    public int PaginaAtual { get; set; }
    public int TamanhoPagina { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}

public class AlunoResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int Idade { get; set; }
    public string GeneroDescricao { get; set; } = string.Empty;
    public bool PossuiDeficiencia { get; set; }
    public string? DeficienciaDescricao { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public bool PossuiMatriculaAtiva { get; set; }
    public string? NomeTurmaAtual { get; set; }
    public int QuantidadeResponsaveis { get; set; }
    public DateTime DataCadastro { get; set; }
}