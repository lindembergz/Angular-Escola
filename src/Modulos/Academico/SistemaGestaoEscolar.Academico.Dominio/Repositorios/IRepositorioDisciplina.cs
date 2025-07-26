using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Academico.Dominio.Repositorios;

public interface IRepositorioDisciplina : IRepository<Disciplina>
{
    Task<Disciplina?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Disciplina>> ObterPorEscolaAsync(Guid escolaId);
    Task<bool> ExisteCodigoNaEscolaAsync(string codigo, Guid escolaId, Guid? excluirId = null);
    
    // Aliases for application layer compatibility
    Task AdicionarAsync(Disciplina disciplina);
    Task<IEnumerable<Disciplina>> ObterTodosAsync();
}