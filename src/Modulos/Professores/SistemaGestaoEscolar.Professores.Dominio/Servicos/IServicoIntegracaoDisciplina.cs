using SistemaGestaoEscolar.Professores.Dominio.DTOs;

namespace SistemaGestaoEscolar.Professores.Dominio.Servicos;

/// <summary>
/// Serviço para integração com o módulo Acadêmico para consultas de disciplinas
/// </summary>
public interface IServicoIntegracaoDisciplina
{
    /// <summary>
    /// Verifica se uma disciplina existe e está ativa
    /// </summary>
    Task<bool> DisciplinaExisteAsync(Guid disciplinaId);
    
    /// <summary>
    /// Obtém informações básicas de uma disciplina
    /// </summary>
    Task<DisciplinaInfoDto?> ObterDisciplinaAsync(Guid disciplinaId);
    
    /// <summary>
    /// Obtém informações de múltiplas disciplinas
    /// </summary>
    Task<IEnumerable<DisciplinaInfoDto>> ObterDisciplinasAsync(IEnumerable<Guid> disciplinaIds);
    
    /// <summary>
    /// Obtém disciplinas disponíveis para uma escola
    /// </summary>
    Task<IEnumerable<DisciplinaInfoDto>> ObterDisciplinasDisponiveisAsync(Guid escolaId);
    
    /// <summary>
    /// Valida se um professor pode lecionar uma disciplina específica
    /// </summary>
    Task<bool> PodeLecionar(Guid professorId, Guid disciplinaId);
}