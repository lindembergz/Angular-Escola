using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Infraestrutura.Mapeadores;
using SistemaGestaoEscolar.Academico.Infraestrutura.Persistencia.Entidades;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Repositorios;

public class RepositorioHorarioMySql : IRepositorioHorario
{
    private readonly DbContext _context;
    private readonly DbSet<HorarioEntity> _horarios;

    public RepositorioHorarioMySql(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _horarios = _context.Set<HorarioEntity>();
    }

    public async Task<Horario?> GetByIdAsync(Guid id)
    {
        return await ObterPorIdAsync(id);
    }

    public async Task<Horario?> ObterPorIdAsync(Guid id)
    {
        var entity = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .FirstOrDefaultAsync(h => h.Id == id);
        
        return entity != null ? HorarioMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<Horario>> GetAllAsync()
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task AddAsync(Horario entity)
    {
        var horarioEntity = HorarioMapper.ToEntity(entity);
        await _horarios.AddAsync(horarioEntity);
    }

    public async Task UpdateAsync(Horario entity)
    {
        var existingEntity = await _horarios.FirstOrDefaultAsync(h => h.Id == entity.Id);
        if (existingEntity != null)
        {
            HorarioMapper.UpdateEntity(existingEntity, entity);
        }
    }

    public async Task DeleteAsync(Horario entity)
    {
        var existingEntity = await _horarios.FirstOrDefaultAsync(h => h.Id == entity.Id);
        if (existingEntity != null)
        {
            _horarios.Remove(existingEntity);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _horarios.AnyAsync(h => h.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _horarios.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Additional methods specific to Horario
    public async Task<IEnumerable<Horario>> ObterPorTurmaAsync(Guid turmaId)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.TurmaId == turmaId)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterPorProfessorAsync(Guid professorId)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.ProfessorId == professorId)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterPorDisciplinaAsync(Guid disciplinaId)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.DisciplinaId == disciplinaId)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterPorAnoLetivoESemestreAsync(int anoLetivo, int semestre)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.AnoLetivo == anoLetivo && h.Semestre == semestre)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterAtivosAsync()
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.Ativo)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterPorDiaSemanaAsync(DayOfWeek diaSemana)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.DiaSemana == (int)diaSemana && h.Ativo)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterPorSalaAsync(string sala)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.Sala == sala && h.Ativo)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<bool> ExisteConflitoAsync(Guid professorId, DayOfWeek diaSemana, 
                                              TimeOnly horaInicio, TimeOnly horaFim, 
                                              int anoLetivo, int semestre, Guid? excluirId = null)
    {
        var query = _horarios.Where(h => 
            h.ProfessorId == professorId &&
            h.DiaSemana == (int)diaSemana &&
            h.AnoLetivo == anoLetivo &&
            h.Semestre == semestre &&
            h.Ativo &&
            h.HoraInicio < horaFim &&
            h.HoraFim > horaInicio);
        
        if (excluirId.HasValue)
            query = query.Where(h => h.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<bool> ExisteConflitoSalaAsync(string sala, DayOfWeek diaSemana, 
                                                   TimeOnly horaInicio, TimeOnly horaFim, 
                                                   int anoLetivo, int semestre, Guid? excluirId = null)
    {
        if (string.IsNullOrEmpty(sala))
            return false;
            
        var query = _horarios.Where(h => 
            h.Sala == sala &&
            h.DiaSemana == (int)diaSemana &&
            h.AnoLetivo == anoLetivo &&
            h.Semestre == semestre &&
            h.Ativo &&
            h.HoraInicio < horaFim &&
            h.HoraFim > horaInicio);
        
        if (excluirId.HasValue)
            query = query.Where(h => h.Id != excluirId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Horario>> ObterGradeHorariaTurmaAsync(Guid turmaId, int anoLetivo, int semestre)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.TurmaId == turmaId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<IEnumerable<Horario>> ObterGradeHorariaProfessorAsync(Guid professorId, int anoLetivo, int semestre)
    {
        var entities = await _horarios
            .Include(h => h.Turma)
            .Include(h => h.Disciplina)
            .Where(h => h.ProfessorId == professorId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync();
        
        return entities.Select(HorarioMapper.ToDomain);
    }

    public async Task<int> ContarAulasPorDisciplinaAsync(Guid disciplinaId, int anoLetivo, int semestre)
    {
        return await _horarios
            .CountAsync(h => h.DisciplinaId == disciplinaId && 
                           h.AnoLetivo == anoLetivo && 
                           h.Semestre == semestre && 
                           h.Ativo);
    }

    public async Task<int> ContarHorasSemanaisProfessorAsync(Guid professorId, int anoLetivo, int semestre)
    {
        var horarios = await _horarios
            .Where(h => h.ProfessorId == professorId && 
                       h.AnoLetivo == anoLetivo && 
                       h.Semestre == semestre && 
                       h.Ativo)
            .ToListAsync();
        
        return horarios.Sum(h => (int)(h.HoraFim - h.HoraInicio).TotalMinutes) / 60;
    }

    // Methods required by domain services
    public async Task<IEnumerable<Horario>> ObterPorPeriodoAsync(int anoLetivo, int semestre)
    {
        return await ObterPorAnoLetivoESemestreAsync(anoLetivo, semestre);
    }

    public async Task<bool> ExisteConflitoProfessorAsync(Guid professorId, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, int anoLetivo, int semestre, Guid? excluirId = null)
    {
        return await ExisteConflitoAsync(professorId, diaSemana, horaInicio, horaFim, anoLetivo, semestre, excluirId);
    }

    public async Task<bool> ExisteConflitoProfessorOuSalaAsync(Guid professorId, string? sala, DayOfWeek diaSemana, TimeOnly horaInicio, TimeOnly horaFim, int anoLetivo, int semestre, Guid? excluirId = null)
    {
        var conflitoProfessor = await ExisteConflitoProfessorAsync(professorId, diaSemana, horaInicio, horaFim, anoLetivo, semestre, excluirId);
        
        if (conflitoProfessor)
            return true;

        if (!string.IsNullOrEmpty(sala))
        {
            var conflitoSala = await ExisteConflitoSalaAsync(sala, diaSemana, horaInicio, horaFim, anoLetivo, semestre, excluirId);
            return conflitoSala;
        }

        return false;
    }

    // Application layer compatibility methods
    public async Task AdicionarAsync(Horario horario)
    {
        await AddAsync(horario);
    }

}