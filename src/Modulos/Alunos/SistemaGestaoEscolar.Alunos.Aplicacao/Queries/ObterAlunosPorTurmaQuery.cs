using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Queries;

public class ObterAlunosPorTurmaQuery : IRequest<ObterAlunosPorTurmaResponse>
{
    public Guid TurmaId { get; set; }
    public int? AnoLetivo { get; set; }
    public bool ApenasAtivos { get; set; } = true;
    public string? FiltroNome { get; set; }
}

public class ObterAlunosPorTurmaResponse
{
    public List<AlunoTurmaDto> Alunos { get; set; } = new();
    public Guid TurmaId { get; set; }
    public string NomeTurma { get; set; } = string.Empty;
    public int AnoLetivo { get; set; }
    public int TotalAlunos { get; set; }
    public int AlunosAtivos { get; set; }
    public int AlunosInativos { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}

public class AlunoTurmaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int Idade { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool Ativo { get; set; }
    public Guid MatriculaId { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; }
    public string StatusMatricula { get; set; } = string.Empty;
    public List<ResponsavelResumoDto> Responsaveis { get; set; } = new();
}

public class ResponsavelResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string TipoDescricao { get; set; } = string.Empty;
    public bool ResponsavelFinanceiro { get; set; }
    public bool ResponsavelAcademico { get; set; }
}