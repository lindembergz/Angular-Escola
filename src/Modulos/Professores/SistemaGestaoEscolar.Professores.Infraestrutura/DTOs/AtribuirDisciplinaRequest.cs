namespace SistemaGestaoEscolar.Professores.Infraestrutura.DTOs;

/// <summary>
/// Request para atribuir disciplina a um professor
/// </summary>
public class AtribuirDisciplinaRequest
{
    /// <summary>
    /// ID da disciplina a ser atribuída
    /// </summary>
    public Guid DisciplinaId { get; set; }
    
    /// <summary>
    /// Carga horária semanal da disciplina para este professor
    /// </summary>
    public int CargaHorariaSemanal { get; set; }
    
    /// <summary>
    /// Observações sobre a atribuição da disciplina
    /// </summary>
    public string? Observacoes { get; set; }
}