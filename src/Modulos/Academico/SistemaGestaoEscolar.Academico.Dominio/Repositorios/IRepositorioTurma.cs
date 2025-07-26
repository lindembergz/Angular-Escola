using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Academico.Dominio.Repositorios;

public interface IRepositorioTurma : IRepository<Turma>
{
    Task<Turma?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Turma>> ObterPorEscolaAsync(Guid escolaId);
    Task<IEnumerable<Turma>> ObterAtivasAsync();
    Task<bool> ExisteNomeNaEscolaAsync(string nome, Guid escolaId, Guid? excluirId = null);
    
    // Aliases for application layer compatibility
    Task AdicionarAsync(Turma turma);
    Task AtualizarAsync(Turma turma);
    Task<IEnumerable<Turma>> ObterTodasPorUnidadeEscolarAsync(Guid unidadeEscolarId);
}