using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.Repositorios;

public interface IRepositorioProfessor
{
    Task<Professor?> ObterPorIdAsync(Guid id);
    Task<Professor?> ObterPorCpfAsync(Cpf cpf);
    Task<Professor?> ObterPorRegistroAsync(string registro);
    Task<IEnumerable<Professor>> ObterPorEscolaAsync(Guid escolaId);
    Task<IEnumerable<Professor>> ObterAtivosAsync();
    Task<IEnumerable<Professor>> ObterPorDisciplinaAsync(Guid disciplinaId);
    Task<bool> ExistePorCpfAsync(Cpf cpf);
    Task<bool> ExistePorRegistroAsync(string registro);
    Task AdicionarAsync(Professor professor);
    Task AtualizarAsync(Professor professor);
    Task RemoverAsync(Professor professor);
    Task<int> ContarPorEscolaAsync(Guid escolaId);
    Task<int> ContarAtivosAsync();
}