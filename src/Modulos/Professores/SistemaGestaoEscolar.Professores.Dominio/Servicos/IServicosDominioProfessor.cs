using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.Servicos;

public interface IServicosDominioProfessor
{
    Task ValidarCpfUnicoAsync(Cpf cpf, Guid? professorIdExcluir = null);
    Task ValidarRegistroUnicoAsync(string registro, Guid? professorIdExcluir = null);
    Task<bool> PodeAtribuirDisciplinaAsync(Guid professorId, Guid disciplinaId);
    Task<int> CalcularCargaHorariaTotalAsync(Guid professorId);
    Task<bool> ProfessorPodeSerDesativadoAsync(Guid professorId);
    Task<IEnumerable<Professor>> ObterProfessoresDisponiveisParaDisciplinaAsync(Guid disciplinaId, Guid escolaId);
    Task<bool> ValidarCapacidadeEscolaAsync(Guid escolaId);
}