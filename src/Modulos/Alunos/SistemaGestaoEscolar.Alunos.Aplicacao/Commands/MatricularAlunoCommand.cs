using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

public class MatricularAlunoCommand : IRequest<MatricularAlunoResponse>
{
    public Guid AlunoId { get; set; }
    public Guid TurmaId { get; set; }
    public int AnoLetivo { get; set; }
    public string? Observacoes { get; set; }
}

public class MatricularAlunoResponse
{
    public Guid MatriculaId { get; set; }
    public Guid AlunoId { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public Guid TurmaId { get; set; }
    public int AnoLetivo { get; set; }
    public string NumeroMatricula { get; set; } = string.Empty;
    public DateTime DataMatricula { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}