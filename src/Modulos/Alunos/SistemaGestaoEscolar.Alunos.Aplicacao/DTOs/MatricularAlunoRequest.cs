namespace SistemaGestaoEscolar.Alunos.Aplicacao.DTOs;

/// <summary>
/// DTO para requisição de matrícula de aluno
/// </summary>
public class MatricularAlunoRequest
{
    /// <summary>
    /// ID da turma como string (será convertido para Guid)
    /// </summary>
    public string TurmaId { get; set; } = string.Empty;
    
    /// <summary>
    /// Ano letivo da matrícula
    /// </summary>
    public int AnoLetivo { get; set; }
    
    /// <summary>
    /// Observações sobre a matrícula (opcional)
    /// </summary>
    public string? Observacoes { get; set; }
}