using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Academico.Dominio.Repositorios;

public interface IRepositorioHorario : IRepository<Horario>
{
    Task<Horario?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<Horario>> ObterPorProfessorAsync(Guid professorId);
    Task<IEnumerable<Horario>> ObterPorDisciplinaAsync(Guid disciplinaId);
    Task<IEnumerable<Horario>> ObterPorPeriodoAsync(int anoLetivo, int semestre);
    Task<bool> ExisteConflitoProfessorAsync(Guid professorId, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, int anoLetivo, int semestre, Guid? excluirId = null);
    Task<bool> ExisteConflitoSalaAsync(string sala, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, int anoLetivo, int semestre, Guid? excluirId = null);
    Task<bool> ExisteConflitoProfessorOuSalaAsync(Guid professorId, string? sala, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, int anoLetivo, int semestre, Guid? excluirId = null);
    
    // Aliases for application layer compatibility
    Task AdicionarAsync(Horario horario);
    Task<IEnumerable<Horario>> ObterPorTurmaAsync(Guid turmaId);
}